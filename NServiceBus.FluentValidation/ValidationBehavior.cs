using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using NServiceBus.Pipeline;

class ValidationBehavior : Behavior<IIncomingLogicalMessageContext>
{
    ValidatorTypeCache validatorTypeCache;

    public ValidationBehavior(ValidatorTypeCache validatorTypeCache)
    {
        this.validatorTypeCache = validatorTypeCache;
    }

    public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        if (validatorTypeCache.TryGetValidators(context, out var buildAll))
        {
            await Validate(context, buildAll).ConfigureAwait(false);
        }
        await next().ConfigureAwait(false);
    }

    static async Task Validate(IIncomingLogicalMessageContext context, IEnumerable<IValidator> buildAll)
    {
        var instance = context.Message.Instance;
        var results = new List<ValidationFailure>();
        foreach (var validator in buildAll)
        {
            if (AsyncValidatorCache.IsAsync(validator))
            {
                var result = await validator.ValidateAsync(instance)
                    .ConfigureAwait(false);
                results.AddRange(result.Errors);
            }
            else
            {
                var result = validator.Validate(instance);
                results.AddRange(result.Errors);
            }
        }

        if (results.Any())
        {
            throw new ValidationException(results);
        }
    }
}