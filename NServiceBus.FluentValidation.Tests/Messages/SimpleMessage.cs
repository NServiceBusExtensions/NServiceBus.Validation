using NServiceBus;

public class SimpleMessage : IMessage
{
    public string Content { get; set; }
}