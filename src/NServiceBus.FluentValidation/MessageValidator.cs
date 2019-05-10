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

    public async Task Validate(Type messageType, IBuilder contextBuilder, object instance, IReadOnlyDictionary<string, string> headers, ContextBag contextBag)
    {
        if (!validatorTypeCache.TryGetValidators(messageType, contextBuilder, out var buildAll))
        {
            return;
        }

        var results = new List<ValidationFailure>();
        var validationContext = new ValidationContext(instance);
        validationContext.RootContextData.Add("Headers", headers);
        validationContext.RootContextData.Add("ContextBag", contextBag);
        foreach (var validator in buildAll)
        {
            if (AsyncValidatorChecker.IsAsync(validator, validationContext))
            {
                var result = await validator.ValidateAsync(validationContext);
                results.AddRange(result.Errors);
            }
            else
            {
                var result = validator.Validate(validationContext);
                results.AddRange(result.Errors);
            }
        }

        if (results.Any())
        {
            throw new MessageValidationException(messageType, results);
        }
    }
}