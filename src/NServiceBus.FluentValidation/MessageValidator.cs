class MessageValidator(TryGetValidators getValidators)
{
    static MethodInfo innerValidateMethod = typeof(MessageValidator).GetMethod(nameof(Validate))!;

    public Task ValidateWithTypeRedirect(Type messageType, IServiceProvider provider, object instance, IReadOnlyDictionary<string, string> headers, ContextBag contextBag)
    {
        var genericMethod = innerValidateMethod.MakeGenericMethod(instance.GetType());
        return (Task) genericMethod.Invoke(
            this,
            [
                messageType,
                provider,
                instance,
                headers,
                contextBag
            ])!;
    }

    public async Task Validate<TMessage>(Type messageType, IServiceProvider provider, TMessage instance, IReadOnlyDictionary<string, string> headers, ContextBag contextBag)
        where TMessage : notnull
    {
        if (typeof(TMessage) == typeof(object))
        {
            throw new("TMessage must not be object");
        }

        if (!getValidators(messageType, provider, out var validators))
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

        if (results.Count != 0)
        {
            throw new MessageValidationException(instance, results);
        }
    }

    static void AddResults(List<TypeValidationFailure> results, ValidationResult result, IValidator validator) =>
        results.AddRange(result.Errors.Select(failure => new TypeValidationFailure(validator.GetType(), failure)));
}