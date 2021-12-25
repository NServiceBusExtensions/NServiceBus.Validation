﻿using NServiceBus.Testing;

public class SagaTests
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
}