using Microsoft.Extensions.DependencyInjection;

class EndpointValidatorTypeCache(Func<Type, IValidator?>? fallback)
{
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = [];

    static Type validatorType = typeof(IValidator<>);

    public bool TryGetValidators(Type messageType, IServiceProvider? provider, out IEnumerable<IValidator> validators)
    {
        var validatorInfo = typeCache.GetOrAdd(
            messageType,
            type =>
            {
                var genericType = validatorType.MakeGenericType(type);
                if (provider != null)
                {
                    var all = provider
                        .GetServices(genericType)
                        .Cast<IValidator>()
                        .ToList();
                    if (all.Count == 0 && fallback != null)
                    {
                        var validator = fallback(messageType);
                        if (validator != null)
                        {
                            all.Add(validator);
                        }
                    }

                    return new(
                        validators: all,
                        hasValidators: all.Count != 0
                    );
                }

                if (fallback != null)
                {
                    var validator = fallback(messageType);
                    if (validator != null)
                    {
                        return new(
                            validators: [validator],
                            hasValidators: true
                        );
                    }
                }

                return new(
                    validators: [],
                    hasValidators: false
                );
            });

        validators = validatorInfo.Validators;
        return validatorInfo.HasValidators;
    }

    class ValidatorInfo(List<IValidator> validators, bool hasValidators)
    {
        public bool HasValidators { get; } = hasValidators;
        public List<IValidator> Validators { get; } = validators;
    }
}