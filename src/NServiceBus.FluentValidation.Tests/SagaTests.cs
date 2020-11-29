using System.Threading.Tasks;
using NServiceBus.Testing;
using Xunit;

public class SagaTests
{
    [Fact]
    public async Task Ensure_saga_data_is_added_to_context()
    {
        MyMessage message = new()
        {
            Content = "a"
        };
        var handlerContext = ValidatingContext.Build(message);
        MySaga.MySagaData sagaData = new();
        MySaga saga = new()
        {
            Data = sagaData
        };
        await handlerContext.Run(saga);
        Assert.Equal(handlerContext.SagaData, sagaData);
    }
}