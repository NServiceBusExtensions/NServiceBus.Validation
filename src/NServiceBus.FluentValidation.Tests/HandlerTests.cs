using System.Threading.Tasks;
using NServiceBus.FluentValidation;
using NServiceBus.Testing;
using Xunit;

public class HandlerTests
{
    [Fact]
    public async Task With_no_validator()
    {
        var handlerContext = new TestableMessageHandlerContext();

        var handler = new MyHandler();
        var message = new MyMessage();
        var results = ValidationFinder.FindValidatorsInAssemblyContaining<MyMessage>();

        await handler.Handle(message, handlerContext);
    }
}