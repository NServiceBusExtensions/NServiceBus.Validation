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

    static MethodInfo innerValidateMethod = typeof(MessageValidator).GetMethod(nameof(Validate))!;

    public Task ValidateWithTypeRedirect(Type messageType, IServiceProvider builder, object instance, IReadOnlyDictionary<string, string> headers, ContextBag contextBag)
    {
        var genericMethod = innerValidateMethod.MakeGenericMethod(instance.GetType());
        return (Task) genericMethod.Invoke(this,
            new[]
            {
                messageType,
                builder,
                instance,
                headers,
                contextBag
            })!;
    }

    public async Task Validate<TMessage>(Type messageType, IServiceProvider builder, TMessage instance, IReadOnlyDictionary<string, string> headers, ContextBag contextBag)
        where TMessage : class
    {
        if (typeof(TMessage) == typeof(object))
        {
            throw new("TMessage must not be object");
        }

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