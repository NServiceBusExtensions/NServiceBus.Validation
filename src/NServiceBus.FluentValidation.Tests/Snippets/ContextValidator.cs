

// ReSharper disable UnusedVariable

#region FluentValidation_ContextValidator
public class ContextValidator :
    AbstractValidator<TheMessage>
{
    public ContextValidator() =>
        RuleFor(_ => _.Content)
            .Custom((propertyValue, validationContext) =>
            {
                var messageHeaders = validationContext.Headers();
                var bag = validationContext.ContextBag();
                if (propertyValue != "User" ||
                    messageHeaders.ContainsKey("Auth"))
                {
                    return;
                }
                validationContext.AddFailure("Expected Auth header to exist");
            });
}
#endregion