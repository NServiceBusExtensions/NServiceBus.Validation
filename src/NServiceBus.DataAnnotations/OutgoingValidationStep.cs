class OutgoingValidationStep :
    RegisterStep
{
    public OutgoingValidationStep()
        :
        base("OutgoingDataAnnotations",
            typeof(OutgoingValidationBehavior),
            "Validates outgoing messages using DataAnnotations",
            _ => new OutgoingValidationBehavior())
    {
        InsertAfterIfExists("ApplyReplyToAddressBehavior");
        InsertAfter("AddHostInfoHeaders");
    }
}