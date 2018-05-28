using NServiceBus;
using NServiceBus.Pipeline;

class ValidationStep : RegisterStep
{
    FluentValidationConfig config;

    public ValidationStep(FluentValidationConfig config) :
        base("FluentValidation", typeof(ValidationBehavior), "Validates message using FluentValidation",
            builder => { return new ValidationBehavior(); })
    {
        this.config = config;
    }
}