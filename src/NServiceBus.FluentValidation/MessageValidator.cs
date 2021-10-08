using FluentValidation;
using FluentValidation.Results;
using NServiceBus.Extensibility;
using NServiceBus.FluentValidation;
using NServiceBus.ObjectBuilder;
using NServiceBus.Pipeline;

class MessageValidator
{
    TryGetValidators tryGetValidators;

    public MessageValidator(TryGetValidators tryGetValidators)
    {
        this.tryGetValidators = tryGetValidators;
    }

    public Task Validate(IInvokeHandlerContext handlerContext)
    {
        return Validate(handlerContext.MessageBeingHandled.GetType(), handlerContext.Builder, handlerContext.MessageBeingHandled, handlerContext.Headers, handlerContext.Extensions);
    }

    public async Task Validate<T>(Type messageType, IBuilder builder, T instance, IReadOnlyDictionary<string, string> headers, ContextBag contextBag)
    {
        if (!tryGetValidators(messageType, builder, out var validators))
        {
            return;
        }

        List<TypeValidationFailure> results = new();
        ValidationContext<T> validationContext = new(instance);
        validationContext.RootContextData.Add("Headers", headers);
        validationContext.RootContextData.Add("ContextBag", contextBag);
        if (validationContext.IsAsync)
        {
            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(validationContext);
                AddResults(results, result, validator);
            }
        }
        else
        {
            foreach (var validator in validators)
            {
                var result = validator.Validate(validationContext);
                AddResults(results, result, validator);
            }
        }

        if (results.Any())
        {
            throw new MessageValidationException(messageType, results);
        }
    }

    static void AddResults(List<TypeValidationFailure> results, ValidationResult result, IValidator validator)
    {
        results.AddRange(result.Errors.Select(failure => new TypeValidationFailure(validator.GetType(), failure)));
    }
}