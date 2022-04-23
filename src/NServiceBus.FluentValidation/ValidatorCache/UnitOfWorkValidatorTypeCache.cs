using FluentValidation;
using NServiceBus.ObjectBuilder;

class UnitOfWorkValidatorTypeCache
{
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = new();

    static Type validatorType = typeof(IValidator<>);
    Func<Type, IValidator?>? fallback;

    public UnitOfWorkValidatorTypeCache(Func<Type, IValidator?>? fallback) =>
        this.fallback = fallback;

    public bool TryGetValidators(Type messageType, IBuilder builder, out IEnumerable<IValidator> validators)
    {
        var validatorInfo = typeCache.GetOrAdd(messageType,
            type => new(validatorType.MakeGenericType(type)));

        if (validatorInfo.HasValidators.HasValue)
        {
            if (!validatorInfo.HasValidators.Value)
            {
                validators = Enumerable.Empty<IValidator>();
                return false;
            }
        }

        validators = builder
            .BuildAll(validatorInfo.ValidatorType)
            .Cast<IValidator>()
            .ToList();

        if (fallback != null && !validators.Any())
        {
            var fallbackValidator = fallback(messageType);
            if (fallbackValidator != null)
            {
                validators = new[] { fallbackValidator };
            }
        }

        var any = validators.Any();
        validatorInfo.HasValidators = any;
        return any;
    }

    class ValidatorInfo
    {
        public ValidatorInfo(Type validatorType) =>
            ValidatorType = validatorType;

        public Type ValidatorType { get; }
        public bool? HasValidators;
    }
}