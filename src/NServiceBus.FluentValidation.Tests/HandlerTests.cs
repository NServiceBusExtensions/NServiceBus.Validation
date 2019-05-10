using System.Threading.Tasks;
using NServiceBus.FluentValidation;
using NServiceBus.Testing;
using Xunit;

public class HandlerTests :
    TestBase
{
    [Fact]
    public async Task Validate_TestableMessageHandlerContext()
    {
        var handlerContext = new TestableMessageHandlerContext();

        var handler = new MyHandler();
        var message = new MyMessage();
        await handler.Handle(message, handlerContext);
        await Assert.ThrowsAsync<MessageValidationException>(() => handlerContext.Validate(message));
    }

    [Fact]
    public async Task Validate_ValidatingHandlerContext()
    {
        var message = new MyMessage();
        var handlerContext = ValidatingContext.Build(message);
        var handler = new MyHandler();
        await Assert.ThrowsAsync<MessageValidationException>(() => handlerContext.Run(handler));
    }

    [Fact]
    public async Task Validate_ValidatingHandlerContext_Static_Run()
    {
        var message = new MyMessage();
        var handler = new MyHandler();
        await Assert.ThrowsAsync<MessageValidationException>(() => ValidatingContext.Run(handler, message));
    }
}

public class TestBase
{
    static TestBase()
    {
        TestContextValidator.AddValidatorsFromAssemblyContaining<MyMessage>();
    }
}