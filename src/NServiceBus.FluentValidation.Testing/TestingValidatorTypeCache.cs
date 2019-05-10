using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NServiceBus.ObjectBuilder;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

class TestingValidatorTypeCache : 
    IValidatorTypeCache
{
    List<Result> validatorScanResults;
    static Type validatorType = typeof(IValidator<>);
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = new ConcurrentDictionary<Type, ValidatorInfo>();

    public TestingValidatorTypeCache(List<Result> validatorScanResults)
    {
        this.validatorScanResults = validatorScanResults;
    }

    public bool TryGetValidators(Type messageType, IBuilder builder, out IEnumerable<IValidator> validators)
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
                return new ValidatorInfo
                {
                    Validators = all,
                    HasValidators = all.Any()
                };
            });

        validators = validatorInfo.Validators;
        return validatorInfo.HasValidators;
    }
}