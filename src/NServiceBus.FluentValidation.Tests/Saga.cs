using System.Threading.Tasks;
using NServiceBus;

class MySaga : Saga<MySaga.MySagaData>,
    IHandleMessages<MyMessage>
{
    public Task Handle(MyMessage message, IMessageHandlerContext context)
    {
        Data.Property = "Value";
        return Task.CompletedTask;
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
    {
    }

    public class MySagaData : ContainSagaData
    {
        public string Property { get; set; }
    }
}