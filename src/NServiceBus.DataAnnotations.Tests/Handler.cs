using NServiceBus;

class Handler :
    IHandleMessages<MessageWithNoValidator>,
    IHandleMessages<MessageWithValidator>
{
    ManualResetEvent resetEvent;

    public Handler(ManualResetEvent resetEvent) =>
        this.resetEvent = resetEvent;

    public Task Handle(MessageWithNoValidator message, HandlerContext context)
    {
        resetEvent.Set();
        return Task.CompletedTask;
    }

    public Task Handle(MessageWithValidator message, HandlerContext context)
    {
        resetEvent.Set();
        return Task.CompletedTask;
    }
}