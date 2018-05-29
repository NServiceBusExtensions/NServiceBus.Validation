using System.Collections.Generic;
using FluentValidation;
using NServiceBus.Pipeline;

internal interface IValidatorTypeCache
{
    bool TryGetValidators(IIncomingLogicalMessageContext context, out IEnumerable<IValidator> buildAll);
}