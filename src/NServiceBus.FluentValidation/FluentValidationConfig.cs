using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus;

public class FluentValidationConfig
{
    internal MessageValidator MessageValidator;

    internal FluentValidationConfig(
        ServiceLifetime lifetime,
        Func<Type, IValidator?>? fallback)
    {
        TryGetValidators tryGetValidators;
        if (lifetime == ServiceLifetime.Singleton)
        {
            tryGetValidators = new EndpointValidatorTypeCache(fallback).TryGetValidators;
        }
        else
        {

            tryGetValidators = new UnitOfWorkValidatorTypeCache(fallback).TryGetValidators;
        }

        MessageValidator = new(tryGetValidators);
    }
}