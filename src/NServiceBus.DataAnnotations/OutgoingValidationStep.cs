using NServiceBus.Pipeline;

class OutgoingValidationStep :
    RegisterStep
{
    public OutgoingValidationStep() :
        base("OutgoingDataAnnotations",
            typeof(OutgoingValidationBehavior),
            "Validates outgoing messages using DataAnnotations",
            builder => new OutgoingValidationBehavior())
    {
        InsertAfterIfExists("ApplyReplyToAddressBehavior");
        InsertAfter("AddHostInfoHeaders");
    }
}