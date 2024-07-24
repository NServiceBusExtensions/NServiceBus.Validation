using NServiceBus.Extensibility;
using VerifyTests.NServiceBus;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus.FluentValidation.Testing;

public static class TestContextValidator
{
    static List<Result> validatorScanResults;
    static MessageValidator validator;

    static TestContextValidator()
    {
        validatorScanResults = [];
        TestingValidatorTypeCache typeCache = new(validatorScanResults);
        validator = new(typeCache.TryGetValidators);
    }

    public static void AddValidatorsFromAssemblyContaining<T>(
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssemblyContaining(typeof(T), throwForNonPublicValidators, throwForNoValidatorsFound);

    public static void AddValidatorsFromAssemblyContaining(
        Type type,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssembly(type.Assembly, throwForNonPublicValidators, throwForNoValidatorsFound);

    public static void AddValidatorsFromAssembly(
        Assembly assembly,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true)
    {
        var results = ValidationFinder.FindValidatorsInAssembly(assembly, throwForNonPublicValidators, throwForNoValidatorsFound);
        AddValidators(results);
    }

    public static void AddValidators(IEnumerable<Result> results) =>
        validatorScanResults.AddRange(results);

    /// <summary>
    /// Register all assemblies matching *.Messages.dll that exist in AppDomain.CurrentDomain.BaseDirectory.
    /// </summary>
    public static void AddValidatorsFromMessagesSuffixedAssemblies() =>
        AddValidators(ValidationFinder.FindValidatorsInMessagesSuffixedAssemblies());

    public static Task Validate<TMessage>(this RecordingHandlerContext context, TMessage message, IServiceProvider? provider = null)
        where TMessage : notnull
    {
        var tasks = new List<Task>
        {
            validator.Validate(message.GetType(), provider, message, context.MessageHeaders, context.Extensions)
        };

        tasks.AddRange(context.Published.Select(_ => ValidateWithTypeRedirect(_.Message, _.Options, provider)));
        tasks.AddRange(context.Sent.Select(_ => ValidateWithTypeRedirect(_.Message, _.Options, provider)));
        tasks.AddRange(context.Replied.Select(_ => ValidateWithTypeRedirect(_.Message, _.Options, provider)));

        return Task.WhenAll(tasks);
    }

    internal static Task ValidateWithTypeRedirect<TOptions>(object instance, TOptions options, IServiceProvider? provider)
        where TOptions : ExtendableOptions =>
        validator.ValidateWithTypeRedirect(instance.GetType(), provider, instance, options.GetHeaders(), options.GetExtensions());

    internal static Task InnerValidate<TMessage>(TMessage instance, IReadOnlyDictionary<string, string> headers, ContextBag contextBag, IServiceProvider? provider)
        where TMessage : class
        => validator.Validate(instance.GetType(), provider, instance, headers, contextBag);
}