using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NServiceBus.Pipeline;

class ValidatorTypeCache
{
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = new ConcurrentDictionary<Type, ValidatorInfo>();

    static Type validatorType = typeof(IValidator<>);

    public bool TryGetValidators(IIncomingLogicalMessageContext context, out IEnumerable<IValidator> buildAll)
    {
        var validatorInfo = typeCache.GetOrAdd(context.Message.MessageType,
            type => new ValidatorInfo
            {
                ValidatorType = validatorType.MakeGenericType(type)
            });

        if (validatorInfo.HasValidators.HasValue)
        {
            if (!validatorInfo.HasValidators.Value)
            {
                buildAll = Enumerable.Empty<IValidator>();
                return false;
            }
        }

        buildAll = context.Builder
            .BuildAll(validatorInfo.ValidatorType)
            .Cast<IValidator>()
            .ToList();

        var any = buildAll.Any();
        validatorInfo.HasValidators = any;
        return any;
    }
}