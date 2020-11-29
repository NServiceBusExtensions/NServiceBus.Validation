using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using NServiceBus.Extensibility;
using NServiceBus.FluentValidation;
using NServiceBus.ObjectBuilder;
using NServiceBus.Pipeline;

class MessageValidator
{
    IValidatorTypeCache validatorTypeCache;

    public MessageValidator(IValidatorTypeCache validatorTypeCache)
    {
        this.validatorTypeCache = validatorTypeCache;
    }

    public Task Validate(IInvokeHandlerContext handlerContext)
    {
        return Validate(handlerContext.MessageBeingHandled.GetType(), handlerContext.Builder, handlerContext.MessageBeingHandled, handlerContext.Headers, handlerContext.Extensions);
    }

    public async Task Validate<T>(Type messageType, IBuilder contextBuilder, T instance, IReadOnlyDictionary<string, string> headers, ContextBag contextBag)
    {
        if (!validatorTypeCache.TryGetValidators(messageType, contextBuilder, out var buildAll))
        {
            return;
        }

        List<TypeValidationFailure> results = new();
        ValidationContext<T> validationContext = new(instance);
        validationContext.RootContextData.Add("Headers", headers);
        validationContext.RootContextData.Add("ContextBag", contextBag);
        foreach (var validator in buildAll)
        {
            IList<ValidationFailure> errors;
            if (validator.IsAsync(validationContext))
            {
                var result = await validator.ValidateAsync(validationContext);
                errors = result.Errors;
            }
            else
            {
                var result = validator.Validate(validationContext);
                errors = result.Errors;
            }

            results.AddRange(errors.Select(failure => new TypeValidationFailure(validator.GetType(), failure)));
        }

        if (results.Any())
        {
            throw new MessageValidationException(messageType, results);
        }
    }
}