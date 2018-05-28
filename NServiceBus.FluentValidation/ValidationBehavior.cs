using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using NServiceBus.Pipeline;

class ValidationBehavior : Behavior<IIncomingLogicalMessageContext>
{
    public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        await Validate(context).ConfigureAwait(false);
        await next().ConfigureAwait(false);
    }

    static async Task Validate(IIncomingLogicalMessageContext context)
    {
        var logicalMessage = context.Message;
        var validatorType = ValidatorTypeCache.Find(logicalMessage.MessageType);
        var contextBuilder = context.Builder;
        var buildAll = contextBuilder.BuildAll(validatorType).Cast<IValidator>().ToList();

        if (!buildAll.Any())
        {
            return;
        }

        var results = new List<ValidationFailure>();
        foreach (var validator in buildAll)
        {
            if (AsyncValidatorCache.IsAsync(validator))
            {
                var result = await validator.ValidateAsync(logicalMessage.Instance)
                    .ConfigureAwait(false);
                results.AddRange(result.Errors);
            }
            else
            {
                var result = validator.Validate(logicalMessage.Instance);
                results.AddRange(result.Errors);
            }
        }

        if (results.Any())
        {
            throw new ValidationException(results);
        }
    }
}