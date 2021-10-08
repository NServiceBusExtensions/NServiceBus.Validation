using VerifyXunit;
using NServiceBus;
using NServiceBus.FluentValidation;
using NServiceBus.Testing;
using Xunit;

[UsesVerify]
public class HandlerTests
{
    [Fact]
    public Task Validate_TestableMessageHandlerContext()
    {
        TestableMessageHandlerContext handlerContext = new();

        MyMessage message = new();
        return Assert.ThrowsAsync<MessageValidationException>(() => handlerContext.Validate(message));
    }

    [Fact]
    public Task Validate_ValidatingHandlerContext()
    {
        MyMessage message = new();
        var handlerContext = ValidatingContext.Build(message);
        MyHandler handler = new();
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
        SimpleMessage message = new();
        var handlerContext = ValidatingContext.Build(message);
        HandlerThatSends handler = new();
        var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(message, handlerContext));
        await Verifier.Verify(exception.Message);
    }

    [Fact]
    public Task Validate_ValidatingHandlerContext_Static_Run()
    {
        MyMessage message = new();
        MyHandler handler = new();
        return Assert.ThrowsAsync<MessageValidationException>(() => ValidatingContext.Run(handler, message));
    }
}