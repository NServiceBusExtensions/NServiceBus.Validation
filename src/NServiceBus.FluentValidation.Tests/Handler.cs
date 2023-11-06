class Handler(ManualResetEvent @event) :
    IHandleMessages<MessageWithNoValidator>,
    IHandleMessages<MessageWithValidator>,
    IHandleMessages<MessageWithAsyncValidator>
{
    public Task Handle(MessageWithNoValidator message, HandlerContext context)
    {
        @event.Set();
        return Task.CompletedTask;
    }

    public Task Handle(MessageWithAsyncValidator message, HandlerContext context)
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