using FluentValidation;
using FluentValidation.Results;
using NServiceBus.Extensibility;
using NServiceBus.FluentValidation;
using NServiceBus.ObjectBuilder;

class MessageValidator
{
    TryGetValidators tryGetValidators;

    public MessageValidator(TryGetValidators tryGetValidators) =>
        this.tryGetValidators = tryGetValidators;

    public async Task Validate<TMessage>(Type messageType, IBuilder builder, TMessage instance, IReadOnlyDictionary<string, string> headers, ContextBag contextBag)
        where TMessage : class
    {
        if (!tryGetValidators(messageType, builder, out var validators))
        {
            return;
        }

        var results = new List<TypeValidationFailure>();
        var validationContext = new ValidationContext<TMessage>(instance);
        validationContext.RootContextData.Add("Headers", headers);
        validationContext.RootContextData.Add("ContextBag", contextBag);
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(validationContext);
            AddResults(results, result, validator);
        }

        if (results.Any())
        {
            throw new MessageValidationException(instance, results);
        }
    }

    static void AddResults(List<TypeValidationFailure> results, ValidationResult result, IValidator validator) =>
        results.AddRange(result.Errors.Select(failure => new TypeValidationFailure(validator.GetType(), failure)));
}