using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NServiceBus.ObjectBuilder;

class EndpointValidatorTypeCache :
    IValidatorTypeCache
{
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = new();

    static Type validatorType = typeof(IValidator<>);

    public bool TryGetValidators(Type messageType, IBuilder builder, out IEnumerable<IValidator> validators)
    {
        var validatorInfo = typeCache.GetOrAdd(
            messageType,
            type =>
            {
                var makeGenericType = validatorType.MakeGenericType(type);
                var all = builder.BuildAll(makeGenericType)
                    .Cast<IValidator>()
                    .ToList();
                return new ValidatorInfo
                (
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