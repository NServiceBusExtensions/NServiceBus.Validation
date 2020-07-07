using System.Collections.Generic;
using System.Linq;
using FluentValidation;

static class AsyncValidatorChecker
{
    public static bool IsAsync(this IValidator validator, IValidationContext context)
    {
        if (validator is IEnumerable<IValidationRule> rules)
        {
            context.RootContextData["__FV_IsAsyncExecution"] = true;
            return rules.Any(validationRule => IsAsync(validationRule, context));
        }

        return false;
    }

    static bool IsAsync(this IValidationRule validationRule, IValidationContext context)
    {
        return validationRule.Validators.Any(validator => validator.ShouldValidateAsynchronously(context));
    }
}