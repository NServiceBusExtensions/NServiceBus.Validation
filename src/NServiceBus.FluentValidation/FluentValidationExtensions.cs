using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Extensibility;
using NServiceBus.FluentValidation;
using NServiceBus.Pipeline;
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

    internal static IServiceProvider GetServiceProvider(this IBehaviorContext context)
    {
        var provider = context.Builder;
        var httpServices = provider.GetService<IHttpContextAccessor>()?.HttpContext?.RequestServices;
        if (httpServices == null)
        {
            return provider;
        }

        return httpServices;
    }

    public static void UseFluentValidation(
        this EndpointConfiguration endpoint,
        bool incoming = true,
        bool outgoing = true,
        Func<Type, IValidator?>? fallback = null)
    {
        var recoverability = endpoint.Recoverability();
        recoverability.AddUnrecoverableException<MessageValidationException>();

        var pipeline = endpoint.Pipeline;

        var messageValidator = GetMessageValidator(fallback);
        if (incoming)
        {
            pipeline.Register(new IncomingValidationStep(messageValidator));
        }

        if (outgoing)
        {
            pipeline.Register(new OutgoingValidationStep(messageValidator));
        }
    }

    static MessageValidator GetMessageValidator(Func<Type, IValidator?>? fallback)
    {
        TryGetValidators tryGetValidators = new EndpointValidatorTypeCache(fallback).TryGetValidators;

        return new(tryGetValidators);
    }

    public static void AddValidators(this IServiceCollection services, IEnumerable<Result> results)
    {
        foreach (var result in results)
        {
            services.AddSingleton(result.InterfaceType, result.ValidatorType);
        }
    }

    public static void AddValidatorsFromAssemblyContaining<T>(
        this IServiceCollection services,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssemblyContaining(services, typeof(T), throwForNonPublicValidators, throwForNoValidatorsFound);

    public static void AddValidatorsFromAssemblyContaining(
        this IServiceCollection services,
        Type type,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssembly(services,  type.Assembly, throwForNonPublicValidators, throwForNoValidatorsFound);

    public static void AddValidatorsFromAssembly(
        this IServiceCollection services,
        Assembly assembly,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true)
    {
        var results = ValidationFinder.FindValidatorsInAssembly(assembly, throwForNonPublicValidators, throwForNoValidatorsFound);
        services.AddValidators(results);
    }

    /// <summary>
    /// Register all assemblies matching *.Messages.dll that exist in AppDomain.CurrentDomain.BaseDirectory.
    /// </summary>
    public static void AddValidatorsFromMessagesSuffixedAssemblies(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        services.AddValidators(ValidationFinder.FindValidatorsInMessagesSuffixedAssemblies(throwForNonPublicValidators, throwForNoValidatorsFound));
}