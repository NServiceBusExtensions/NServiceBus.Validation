using FluentValidation;
using NServiceBus.Extensibility;

namespace NServiceBus;

/// <summary>
/// Extensions to FluentValidation.
/// </summary>
public static class FluentValidationExtensions
{
    public static IReadOnlyDictionary<string, string> Headers(this IValidationContext validationContext)
    {
        return (IReadOnlyDictionary<string, string>)validationContext.RootContextData["Headers"];
    }

    public static ContextBag ContextBag(this IValidationContext validationContext)
    {
        return (ContextBag)validationContext.RootContextData["ContextBag"];
    }
}