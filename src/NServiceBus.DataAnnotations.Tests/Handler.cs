class Handler(ManualResetEvent @event) :
    IHandleMessages<MessageWithNoValidator>,
    IHandleMessages<MessageWithValidator>
{
    public Task Handle(MessageWithNoValidator message, HandlerContext context)
    {
        @event.Set();
        return Task.CompletedTask;
    }

    public Task Handle(MessageWithValidator message, HandlerContext context)
    {
        @event.Set();
        return Task.CompletedTask;
    }
}