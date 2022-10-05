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

    public static void AddValidatorsFromAssemblyContaining<T>(
        IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssemblyContaining(services, typeof(T),lifetime,  throwForNonPublicValidators, throwForNoValidatorsFound);

    public static void AddValidatorsFromAssemblyContaining(
        IServiceCollection services,
        Type type,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssembly(services,  type.Assembly, lifetime, throwForNonPublicValidators, throwForNoValidatorsFound);

    public static void AddValidatorsFromAssembly(
        IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true)
    {
        var results = ValidationFinder.FindValidatorsInAssembly(assembly, throwForNonPublicValidators, throwForNoValidatorsFound);
        services.AddValidators(results, lifetime);
    }

    /// <summary>
    /// Register all assemblies matching *.Messages.dll that exist in AppDomain.CurrentDomain.BaseDirectory.
    /// </summary>
    public static void AddValidatorsFromMessagesSuffixedAssemblies(
        IServiceCollection services,
        ServiceLifetime lifetime,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        services.AddValidators(ValidationFinder.FindValidatorsInMessagesSuffixedAssemblies(throwForNonPublicValidators, throwForNoValidatorsFound), lifetime);
}