using NServiceBus;
using NServiceBus.Pipeline;

class ValidationStep : RegisterStep
{
    FluentValidationConfig config;

    public ValidationStep(FluentValidationConfig config) :
        base("FluentValidation", typeof(ValidationBehavior), "Validates message using FluentValidation",
            builder =>
            {
                if (config.validatorLifecycle == ValidatorLifecycle.Endpoint)
                {
                    return new ValidationBehavior(new EndpointValidatorTypeCache());
                }
                return new ValidationBehavior(new UnitOfWorkValidatorTypeCache());
            })
    {
        this.config = config;
    }
}