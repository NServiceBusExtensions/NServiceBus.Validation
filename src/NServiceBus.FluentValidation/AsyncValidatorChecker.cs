using System.Collections.Generic;
using System.Linq;
using FluentValidation;

static class AsyncValidatorChecker
{
    public static bool IsAsync(IValidator validator, ValidationContext context)
    {
        if (validator is IEnumerable<IValidationRule> rules)
        {
            context.RootContextData["__FV_IsAsyncExecution"] = true;
            return rules.Any(validationRule => IsAsync(validationRule, context));
        }

        return false;
    }

    static bool IsAsync(IValidationRule validationRule, ValidationContext context)
    {
        return validationRule.Validators.Any(validator => validator.ShouldValidateAsync(context));
    }
}