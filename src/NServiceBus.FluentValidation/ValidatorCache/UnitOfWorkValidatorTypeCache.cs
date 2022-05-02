using FluentValidation;
using NServiceBus.ObjectBuilder;

class UnitOfWorkValidatorTypeCache
{
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = new();

    static Type validatorType = typeof(IValidator<>);
    Func<Type, IValidator?>? fallback;

    public UnitOfWorkValidatorTypeCache(Func<Type, IValidator?>? fallback) =>
        this.fallback = fallback;

    public CacheResult TryGetValidators(Type messageType, IBuilder builder)
    {
        var validatorInfo = typeCache.GetOrAdd(messageType,
            type => new(validatorType.MakeGenericType(type)));

        if (validatorInfo.HasValidators.HasValue)
        {
            if (!validatorInfo.HasValidators.Value)
            {
                return new (Array.Empty<IValidator>());
            }
        }

        var validators = builder
            .BuildAll(validatorInfo.ValidatorType)
            .Cast<IValidator>()
            .ToList();

        if (fallback != null && !validators.Any())
        {
            var fallbackValidator = fallback(messageType);
            if (fallbackValidator != null)
            {
                validators = new()
                    { fallbackValidator };
            }
        }

        validatorInfo.HasValidators = validators.Any();
        return new(validators);
    }

    class ValidatorInfo
    {
        public ValidatorInfo(Type validatorType) =>
            ValidatorType = validatorType;

        public Type ValidatorType { get; }
        public bool? HasValidators;
    }
}