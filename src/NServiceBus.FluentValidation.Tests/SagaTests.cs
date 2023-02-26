using NServiceBus.Testing;

public class SagaTests
{
    [Test]
    public async Task Ensure_saga_data_is_added_to_context()
    {
        var message = new MyMessage
        {
            Content = "a"
        };

        var handlerContext = ValidatingContext.Build(message, new FakeServiceProvider());
        var sagaData = new MySaga.MySagaData();
        var saga = new MySaga
        {
            Data = sagaData
        };
        await handlerContext.Run(saga);
        Assert.AreEqual(handlerContext.SagaData, sagaData);
    }
}