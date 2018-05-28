using System;
using System.Collections.Generic;

public class AuditedMessageData
{
    public Guid MessageId { get; set; }
    public Type MessageType { get; set; }
    public string ProcessingEndpoint { get; set; }
    public string OriginatingEndpoint { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    public string Body;
}