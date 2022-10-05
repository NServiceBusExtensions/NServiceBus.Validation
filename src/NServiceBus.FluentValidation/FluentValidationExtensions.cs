using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Extensibility;
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
}