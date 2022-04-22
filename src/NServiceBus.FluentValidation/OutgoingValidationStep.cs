using NServiceBus;
using NServiceBus.Pipeline;

class OutgoingValidationStep : RegisterStep
{
    public OutgoingValidationStep(FluentValidationConfig config) :
        base(
            stepId: "OutgoingFluentValidation",
            behavior: typeof(OutgoingValidationBehavior),
            description: "Validates outgoing messages using FluentValidation",
            factoryMethod: _ => BuildBehavior(config))
    {
    }

    static IBehavior BuildBehavior(FluentValidationConfig config) =>
        new OutgoingValidationBehavior(config.MessageValidator);
}