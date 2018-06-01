using NServiceBus;
using NServiceBus.Pipeline;

class ValidationStep : RegisterStep
{
    public ValidationStep(FluentValidationConfig config) :
        base("FluentValidation", typeof(ValidationBehavior), "Validates message using FluentValidation",
            builder =>
            {
                if (config.validatorLifecycle == ValidatorLifecycle.Endpoint)
                {
                    return new ValidationBehavior(new MessageValidator(new EndpointValidatorTypeCache()));
                }
                return new ValidationBehavior(new MessageValidator(new UnitOfWorkValidatorTypeCache()));
            })
    {
    }
}