using FluentValidation;
using NServiceBus.ObjectBuilder;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

class TestingValidatorTypeCache
{
    List<Result> validatorScanResults;
    static Type validatorType = typeof(IValidator<>);
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = new();

    public TestingValidatorTypeCache(List<Result> validatorScanResults) =>
        this.validatorScanResults = validatorScanResults;

    public CacheResult TryGetValidators(Type messageType, IBuilder builder)
    {
        var validatorInfo = typeCache.GetOrAdd(
            messageType,
            type =>
            {
                var messageValidatorType = validatorType.MakeGenericType(type);
                var all = validatorScanResults
                    .Where(x => x.InterfaceType.IsAssignableFrom(messageValidatorType))
                    .Select(x => Activator.CreateInstance(x.ValidatorType))
                    .Cast<IValidator>()
                    .ToList();
                return new(
                    validators: all,
                    hasValidators: all.Any()
                );
            });

        return new(validatorInfo.Validators);
    }
}