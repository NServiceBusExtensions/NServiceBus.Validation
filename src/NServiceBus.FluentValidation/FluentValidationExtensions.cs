using FluentValidation;
using NServiceBus.Extensibility;

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
}