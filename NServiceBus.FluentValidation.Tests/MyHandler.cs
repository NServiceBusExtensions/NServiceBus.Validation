using System.Threading.Tasks;
using NServiceBus;

class MyHandler :
    IHandleMessages<MessageWithIncludeInAudit>,
    IHandleMessages<MessageWithExcludeFromAudit>,
    IHandleMessages<SimpleMessage>
{
    public Task Handle(MessageWithIncludeInAudit message, IMessageHandlerContext context)
    {
        return Task.FromResult(0);
    }

    public Task Handle(MessageWithExcludeFromAudit message, IMessageHandlerContext context)
    {
        return Task.FromResult(0);
    }

    public Task Handle(SimpleMessage message, IMessageHandlerContext context)
    {
        return Task.FromResult(0);
    }
}