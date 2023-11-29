using NServiceBus.Testing;

public class HandlerTests
{
    IServiceProvider provider = new FakeServiceProvider();

    [Test]
    public void Validate_TestableMessageHandlerContext()
    {
        var handlerContext = new TestableMessageHandlerContext();

        var message = new MyMessage();
        ThrowsAsync<MessageValidationException>(() => handlerContext.Validate(message, new FakeServiceProvider()));
    }

    [Test]
    public void Validate_ValidatingHandlerContext()
    {
        var message = new MyMessage();
        var handlerContext = ValidatingContext.Build(message, provider);
        var handler = new MyHandler();
        ThrowsAsync<MessageValidationException>(() => handlerContext.Run(handler));
    }

    class SimpleMessage :
        IMessage;

    class HandlerThatSends :
        IHandleMessages<SimpleMessage>
    {
        public Task Handle(SimpleMessage message, HandlerContext context) =>
            context.SendLocal(new SimpleMessage());
    }

    [Test]
    public async Task Should_throw_for_handle()
    {
        var message = new SimpleMessage();
        var handlerContext = ValidatingContext.Build(message, provider);
        var handler = new HandlerThatSends();
        var exception = ThrowsAsync<Exception>(() => handler.Handle(message, handlerContext))!;
        await Verify(exception.Message);
    }

    [Test]
    public void Validate_ValidatingHandlerContext_Static_Run()
    {
        var message = new MyMessage();
        var handler = new MyHandler();
        ThrowsAsync<MessageValidationException>(() => ValidatingContext.Run(handler, message, provider));
    }
}

class FakeServiceProvider : IServiceProvider
{
    public object? GetService(Type serviceType) =>
        null;
}