using NServiceBus;
using NServiceBus.Pipeline;

class IncomingValidationStep : RegisterStep
{
    public IncomingValidationStep(FluentValidationConfig config) :
        base("IncomingFluentValidation", typeof(IncomingValidationBehavior), "Validates incoming messages using FluentValidation",
            builder =>
            {
                if (config.validatorLifecycle == ValidatorLifecycle.Endpoint)
                {
                    return new IncomingValidationBehavior(new MessageValidator(new EndpointValidatorTypeCache()));
                }
                return new IncomingValidationBehavior(new MessageValidator(new UnitOfWorkValidatorTypeCache()));
            })
    {
    }
}