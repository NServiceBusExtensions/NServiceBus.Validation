using NServiceBus.Pipeline;

class ValidationStep : RegisterStep
{
    public ValidationStep() :
        base("DataAnnotations", typeof(ValidationBehavior), "Validates message using DataAnnotations",
            builder => new ValidationBehavior())
    {
    }
}