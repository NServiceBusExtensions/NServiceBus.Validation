using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

class TestingValidatorTypeCache(List<Result> scanResults)
{
    static Type validatorType = typeof(IValidator<>);
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = [];

    public bool TryGetValidators(Type messageType, IServiceProvider builder, out IEnumerable<IValidator> validators)
    {
        var validatorInfo = typeCache.GetOrAdd(
            messageType,
            type =>
            {
                var messageValidatorType = validatorType.MakeGenericType(type);
                var all = scanResults
                    .Where(_ => _.InterfaceType.IsAssignableFrom(messageValidatorType))
                    .Select(_ => Activator.CreateInstance(_.ValidatorType))
                    .Cast<IValidator>()
                    .ToList();
                return new(
                    validators: all,
                    hasValidators: all.Any()
                );
            });

        validators = validatorInfo.Validators;
        return validatorInfo.HasValidators;
    }
}