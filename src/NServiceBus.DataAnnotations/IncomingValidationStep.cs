using NServiceBus.Pipeline;

class IncomingValidationStep :
    RegisterStep
{
    public IncomingValidationStep(IServiceProvider provider) :
        base(
            "IncomingDataAnnotations",
            typeof(IncomingValidationBehavior),
            "Validates incoming messages using DataAnnotations",
            _ => new IncomingValidationBehavior(provider)) =>
        InsertAfter("MutateIncomingMessages");
}