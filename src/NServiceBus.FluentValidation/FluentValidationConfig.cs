using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.FluentValidation;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus;

public class FluentValidationConfig
{
    IServiceCollection services;
    ServiceLifetime lifetime;
    internal MessageValidator MessageValidator;

    internal FluentValidationConfig(
        IServiceCollection services,
        ServiceLifetime lifetime,
        Func<Type, IValidator?>? fallback)
    {
        this.services = services;
        this.lifetime = lifetime;

        MessageValidator = new(GetValidatorCache(fallback));
    }

    TryGetValidators GetValidatorCache(Func<Type, IValidator?>? fallback)
    {
        if (lifetime == ServiceLifetime.Singleton)
        {
            return new EndpointValidatorTypeCache(fallback).TryGetValidators;
        }

        return new UnitOfWorkValidatorTypeCache(fallback).TryGetValidators;
    }

    public void AddValidatorsFromAssemblyContaining<T>(
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssemblyContaining(typeof(T), throwForNonPublicValidators, throwForNoValidatorsFound);

    public void AddValidatorsFromAssemblyContaining(
        Type type, bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssembly(type.Assembly, throwForNonPublicValidators, throwForNoValidatorsFound);

    public void AddValidatorsFromAssembly(
        Assembly assembly,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true)
    {
        var results = ValidationFinder.FindValidatorsInAssembly(assembly, throwForNonPublicValidators, throwForNoValidatorsFound);
        services.AddValidators(lifetime, results);
    }
    /// <summary>
    /// Register all assemblies matching *.Messages.dll that exist in AppDomain.CurrentDomain.BaseDirectory.
    /// </summary>
    public void AddValidatorsFromMessagesSuffixedAssemblies(
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        services.AddValidators(lifetime, ValidationFinder.FindValidatorsInMessagesSuffixedAssemblies(throwForNonPublicValidators, throwForNoValidatorsFound));
}