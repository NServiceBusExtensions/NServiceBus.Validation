using NServiceBus.Pipeline;

class IncomingValidationStep :
    RegisterStep
{
    public IncomingValidationStep(MessageValidator validator) :
        base(
            stepId: "IncomingFluentValidation",
            behavior: typeof(IncomingValidationBehavior),
            description: "Validates incoming messages using FluentValidation",
            factoryMethod: _ => new IncomingValidationBehavior(validator))
    {
    }
}