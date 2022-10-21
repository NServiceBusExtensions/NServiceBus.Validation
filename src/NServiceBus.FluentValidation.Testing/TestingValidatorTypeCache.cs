using FluentValidation;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

class TestingValidatorTypeCache
{
    List<Result> validatorScanResults;
    static Type validatorType = typeof(IValidator<>);
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = new();

    public TestingValidatorTypeCache(List<Result> validatorScanResults) =>
        this.validatorScanResults = validatorScanResults;

    public bool TryGetValidators(Type messageType, IServiceProvider builder, out IEnumerable<IValidator> validators)
    {
        var validatorInfo = typeCache.GetOrAdd(
            messageType,
            type =>
            {
                var messageValidatorType = validatorType.MakeGenericType(type);
                var all = validatorScanResults
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