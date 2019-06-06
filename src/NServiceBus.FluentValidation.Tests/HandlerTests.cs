using System.Threading.Tasks;
using NServiceBus.FluentValidation;
using NServiceBus.Testing;
using Xunit;

public class HandlerTests :
    TestBase
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

    [Fact]
    public Task Validate_ValidatingHandlerContext_Static_Run()
    {
        var message = new MyMessage();
        var handler = new MyHandler();
        return Assert.ThrowsAsync<MessageValidationException>(() => ValidatingContext.Run(handler, message));
    }
}