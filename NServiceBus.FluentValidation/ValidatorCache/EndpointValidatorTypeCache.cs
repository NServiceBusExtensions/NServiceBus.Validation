using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NServiceBus.Pipeline;

class EndpointValidatorTypeCache : IValidatorTypeCache
{
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = new ConcurrentDictionary<Type, ValidatorInfo>();

    static Type validatorType = typeof(IValidator<>);

    public bool TryGetValidators(IIncomingLogicalMessageContext context, out IEnumerable<IValidator> buildAll)
    {
        var validatorInfo = typeCache.GetOrAdd(context.Message.MessageType,
            type =>
            {
                var makeGenericType = validatorType.MakeGenericType(type);
                var all = context.Builder.BuildAll(makeGenericType)
                    .Cast<IValidator>()
                    .ToList();
                return new ValidatorInfo
                {
                    Validators = all,
                    HasValidators = all.Any()
                };
            });


        buildAll = validatorInfo.Validators;
        return validatorInfo.HasValidators;
    }

    class ValidatorInfo
    {
        public bool HasValidators;
        public List<IValidator> Validators;
    }
}