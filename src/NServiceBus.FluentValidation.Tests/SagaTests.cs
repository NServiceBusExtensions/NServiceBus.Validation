using System.Threading.Tasks;
using NServiceBus.Testing;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class SagaTests :
    VerifyBase
{
    [Fact]
    public async Task Ensure_saga_data_is_added_to_context()
    {
        var message = new MyMessage
        {
            Content = "a"
        };
        var handlerContext = ValidatingContext.Build(message);
        var sagaData = new MySaga.MySagaData();
        var saga = new MySaga
        {
            Data = sagaData
        };
        await handlerContext.Run(saga);
        Assert.Equal(handlerContext.SagaData, sagaData);
    }

    public SagaTests(ITestOutputHelper output) :
        base(output)
    {
    }
}