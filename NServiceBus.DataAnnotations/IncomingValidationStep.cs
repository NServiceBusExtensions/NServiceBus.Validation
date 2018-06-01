using NServiceBus.Pipeline;

class IncomingValidationStep : RegisterStep
{
    public IncomingValidationStep() :
        base("IncomingDataAnnotations", typeof(IncomingValidationBehavior), "Validates incoming message using DataAnnotations",
            builder => new IncomingValidationBehavior())
    {
    }
}