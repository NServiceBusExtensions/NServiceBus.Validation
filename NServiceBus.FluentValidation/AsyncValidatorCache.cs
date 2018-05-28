using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

static class AsyncValidatorCache
{
    static ConcurrentDictionary<Type, bool> typeCache = new ConcurrentDictionary<Type, bool>();

    public static bool IsAsync(IValidator validator)
    {
        return typeCache.GetOrAdd(validator.GetType(), type => CheckIfAsync(validator));
    }

    static bool CheckIfAsync(IValidator validator)
    {
        if (validator is IEnumerable<IValidationRule> rules)
        {
            if (rules.Any(validationRule => validationRule.Validators
                .Any(propertyValidator => propertyValidator.IsAsync)))
            {
                return true;
            }
        }

        return false;
    }
}