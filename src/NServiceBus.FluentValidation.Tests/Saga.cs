using NServiceBus;

class MySaga :
    Saga<MySaga.MySagaData>,
    IHandleMessages<MyMessage>
{
    public Task Handle(MyMessage message, HandlerContext context)
    {
        Data.Property = "Value";
        return Task.CompletedTask;
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
    {
    }

    public class MySagaData :
        ContainSagaData
    {
        public string? Property { get; set; }
    }
}