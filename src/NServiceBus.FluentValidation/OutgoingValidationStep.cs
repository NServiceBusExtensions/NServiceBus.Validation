using NServiceBus.Pipeline;

class OutgoingValidationStep : RegisterStep
{
    public OutgoingValidationStep(MessageValidator validator) :
        base(
            stepId: "OutgoingFluentValidation",
            behavior: typeof(OutgoingValidationBehavior),
            description: "Validates outgoing messages using FluentValidation",
            factoryMethod: _ => new OutgoingValidationBehavior(validator))
    {
    }
}