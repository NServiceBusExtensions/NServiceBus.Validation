using System;
using System.Collections.Generic;
using FluentValidation;
using NServiceBus.ObjectBuilder;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus.Testing
{
    public class TestingValidatorTypeCache : IValidatorTypeCache
    {
        List<Result> validatorScanResults = new List<Result>();
        public bool TryGetValidators(Type messageType, IBuilder builder, out IEnumerable<IValidator> validators)
        {
            throw new NotImplementedException();
        }
    }
}
