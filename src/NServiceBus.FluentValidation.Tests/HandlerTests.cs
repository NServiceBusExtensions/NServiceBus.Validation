using NServiceBus;
using NServiceBus.FluentValidation;
using NServiceBus.Testing;

[UsesVerify]
public class HandlerTests
{
    [Fact]
    public Task Validate_TestableMessageHandlerContext()
    {
        var handlerContext = new TestableMessageHandlerContext();

        var message = new MyMessage();
        return Assert.ThrowsAsync<MessageValidationException>(() => handlerContext.Validate(message));
    }

    [Fact]
    public Task Validate_ValidatingHandlerContext()
    {
        var message = new MyMessage();
        var handlerContext = ValidatingContext.Build(message);
        var handler = new MyHandler();
        return Assert.ThrowsAsync<MessageValidationException>(() => handlerContext.Run(handler));
    }

    class SimpleMessage :
        IMessage
    {
    }

    class HandlerThatSends :
        IHandleMessages<SimpleMessage>
    {
        public Task Handle(SimpleMessage message, IMessageHandlerContext context)
        {
            return context.SendLocal(new SimpleMessage());
        }
    }

    [Fact]
    public async Task Should_throw_for_handle()
    {
        var message = new SimpleMessage();
        var handlerContext = ValidatingContext.Build(message);
        var handler = new HandlerThatSends();
        var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(message, handlerContext));
        await Verify(exception.Message);
    }

    [Fact]
    public Task Validate_ValidatingHandlerContext_Static_Run()
    {
        var message = new MyMessage();
        var handler = new MyHandler();
        return Assert.ThrowsAsync<MessageValidationException>(() => ValidatingContext.Run(handler, message));
    }
}