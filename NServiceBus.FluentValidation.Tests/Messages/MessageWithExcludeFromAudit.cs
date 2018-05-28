using NServiceBus;
using NServiceBus.AuditFilter;

[ExcludeFromAudit]
public class MessageWithExcludeFromAudit : IMessage
{
    public string Content { get; set; }
}