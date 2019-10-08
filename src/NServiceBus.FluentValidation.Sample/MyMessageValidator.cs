using FluentValidation;
using FluentValidation.Results;
using NServiceBus;
// ReSharper disable UnusedVariable

public class MyMessageValidator :
    AbstractValidator<MyMessage>
{
    public override ValidationResult Validate(ValidationContext<MyMessage> context)
    {
        var validationResult = base.Validate(context);
        return validationResult;
    }

    public MyMessageValidator()
    {
        RuleFor(_ => _.Content)
            .NotEmpty()
            .Custom((propertyValue, validationContext) =>
            {
                var pipelineContextBag = validationContext.ContextBag();
                var messageHeaders = validationContext.Headers();
                if (propertyValue == "User" &&
                    messageHeaders.ContainsKey("Auth"))
                {
                    validationContext.AddFailure("D");
                }
            });
    }
}