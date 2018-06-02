using NServiceBus;
using NServiceBus.Pipeline;

class OutgoingValidationStep : RegisterStep
{
    public OutgoingValidationStep(FluentValidationConfig config) :
        base("OutgoingFluentValidation", typeof(OutgoingValidationBehavior), "Validates outgoing messages using FluentValidation",
            builder =>
            {
                if (config.validatorLifecycle == ValidatorLifecycle.Endpoint)
                {
                    return new OutgoingValidationBehavior(new MessageValidator(new EndpointValidatorTypeCache()));
                }
                return new OutgoingValidationBehavior(new MessageValidator(new UnitOfWorkValidatorTypeCache()));
            })
    {
    }
}