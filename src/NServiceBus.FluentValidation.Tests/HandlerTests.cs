using System.Threading.Tasks;
using NServiceBus.FluentValidation;
using NServiceBus.Testing;
using Xunit;

public class HandlerTests: TestBase
{
    [Fact]
    public async Task With_no_validator()
    {
        var handlerContext = new TestableMessageHandlerContext();

        var handler = new MyHandler();
        var message = new MyMessage();
        await handler.Handle(message, handlerContext);
        await Assert.ThrowsAsync<MessageValidationException>(()=> handlerContext.RunValidators(message));
    }
}

public class TestBase
{
    static TestBase()
    {
        TestContextValidator.AddValidatorsFromAssemblyContaining<MyMessage>();
    }
}