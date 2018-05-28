using NServiceBus;
using NServiceBus.AuditFilter;

[IncludeInAudit]
public class MessageWithIncludeInAudit : IMessage
{
    public string Content { get; set; }
}