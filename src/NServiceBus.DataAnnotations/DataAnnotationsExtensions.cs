namespace NServiceBus;

/// <summary>
/// Extensions to DataAnnotations.
/// </summary>
public static class DataAnnotationsExtensions
{
    public static IReadOnlyDictionary<string, string> Headers(this ValidationContext validationContext) =>
        (IReadOnlyDictionary<string, string>) validationContext.Items["Headers"]!;

    public static ContextBag ContextBag(this ValidationContext validationContext) =>
        (ContextBag) validationContext.Items["ContextBag"]!;
}