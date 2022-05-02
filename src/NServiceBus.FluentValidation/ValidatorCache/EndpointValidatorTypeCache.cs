using FluentValidation;
using NServiceBus.ObjectBuilder;

class EndpointValidatorTypeCache
{
    Func<Type, IValidator?>? fallback;
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = new();

    static Type validatorType = typeof(IValidator<>);

    public EndpointValidatorTypeCache(Func<Type, IValidator?>? fallback) =>
        this.fallback = fallback;

    public CacheResult TryGetValidators(Type messageType, IBuilder builder)
    {
        var validatorInfo = typeCache.GetOrAdd(
            messageType,
            type =>
            {
                var genericType = validatorType.MakeGenericType(type);
                var all = builder.BuildAll(genericType)
                    .Cast<IValidator>()
                    .ToList();
                if (fallback != null && !all.Any())
                {
                    var fallbackValidator = fallback(messageType);
                    if (fallbackValidator != null)
                    {
                        all.Add(fallbackValidator);
                    }
                }

                return new(validators: all);
            });

        return new(validatorInfo.Validators);
    }

    class ValidatorInfo
    {
        public List<IValidator> Validators { get; }

        public ValidatorInfo(List<IValidator> validators) =>
            Validators = validators;
    }
}