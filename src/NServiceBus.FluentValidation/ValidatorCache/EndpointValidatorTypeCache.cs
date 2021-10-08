using FluentValidation;
using NServiceBus.ObjectBuilder;

class EndpointValidatorTypeCache
{
    Func<Type, IValidator?>? fallback;
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = new();

    static Type validatorType = typeof(IValidator<>);

    public EndpointValidatorTypeCache(Func<Type, IValidator?>? fallback)
    {
        this.fallback = fallback;
    }

    public bool TryGetValidators(Type messageType, IBuilder builder, out IEnumerable<IValidator> validators)
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

                return new(
                    validators: all,
                    hasValidators: all.Any()
                );
            });

        validators = validatorInfo.Validators;
        return validatorInfo.HasValidators;
    }

    class ValidatorInfo
    {
        public bool HasValidators { get; }
        public List<IValidator> Validators { get; }

        public ValidatorInfo(List<IValidator> validators, bool hasValidators)
        {
            Validators = validators;
            HasValidators = hasValidators;
        }
    }
}