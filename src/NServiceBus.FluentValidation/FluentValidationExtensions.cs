using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Extensibility;
using NServiceBus.FluentValidation;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus;

/// <summary>
/// Extensions to FluentValidation.
/// </summary>
public static class FluentValidationExtensions
{
    public static IReadOnlyDictionary<string, string> Headers(this IValidationContext context) =>
        (IReadOnlyDictionary<string, string>)context.RootContextData["Headers"];

    public static ContextBag ContextBag(this IValidationContext context) =>
        (ContextBag)context.RootContextData["ContextBag"];

    public static void AddValidators(this IServiceCollection services, IEnumerable<Result> results, ServiceLifetime lifetime)
    {
        foreach (var result in results)
        {
            services.Add(new(result.InterfaceType, result.ValidatorType, lifetime));
        }
    }

    public static void AddValidatorsFromAssemblyContaining<T>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssemblyContaining(services, typeof(T), lifetime, throwForNonPublicValidators, throwForNoValidatorsFound);

    public static void AddValidatorsFromAssemblyContaining(
        this IServiceCollection services,
        Type type,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssembly(services,  type.Assembly, lifetime, throwForNonPublicValidators, throwForNoValidatorsFound);

    public static void AddValidatorsFromAssembly(
        this IServiceCollection services,
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
        this IServiceCollection services,
        ServiceLifetime lifetime,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        services.AddValidators(ValidationFinder.FindValidatorsInMessagesSuffixedAssemblies(throwForNonPublicValidators, throwForNoValidatorsFound), lifetime);
}