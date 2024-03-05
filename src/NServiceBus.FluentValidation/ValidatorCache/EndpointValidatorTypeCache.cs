using Microsoft.Extensions.DependencyInjection;

class EndpointValidatorTypeCache(Func<Type, IValidator?>? fallback)
{
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = [];

    static Type validatorType = typeof(IValidator<>);

    public bool TryGetValidators(Type messageType, IServiceProvider provider, out IEnumerable<IValidator> validators)
    {
        var validatorInfo = typeCache.GetOrAdd(
            messageType,
            type =>
            {
                var genericType = validatorType.MakeGenericType(type);
                var all = provider
                    .GetServices(genericType)
                    .Cast<IValidator>()
                    .ToList();
                if (fallback != null && all.Count == 0)
                {
                    var fallbackValidator = fallback(messageType);
                    if (fallbackValidator != null)
                    {
                        all.Add(fallbackValidator);
                    }
                }

                return new(
                    validators: all,
                    hasValidators: all.Count != 0
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