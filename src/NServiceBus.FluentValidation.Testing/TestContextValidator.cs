using NServiceBus.Extensibility;
using NServiceBus.FluentValidation;
using NServiceBus.ObjectBuilder;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus.Testing;

public static class TestContextValidator
{
    static List<Result> validatorScanResults;
    static MessageValidator validator;

    static TestContextValidator()
    {
        validatorScanResults = new();
        TestingValidatorTypeCache typeCache = new(validatorScanResults);
        validator = new(typeCache.TryGetValidators);
    }

    public static void AddValidatorsFromAssemblyContaining<T>(bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssemblyContaining(typeof(T), throwForNonPublicValidators, throwForNoValidatorsFound);

    public static void AddValidatorsFromAssemblyContaining(Type type, bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssembly(type.Assembly, throwForNonPublicValidators, throwForNoValidatorsFound);

    public static void AddValidatorsFromAssembly(Assembly assembly, bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true)
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

    public static Task Validate<TMessage>(this TestableMessageHandlerContext context, TMessage message)
        where TMessage : class
    {
        var builder = context.Builder;
        List<Task> tasks = new()
        {
            validator.Validate(message.GetType(), builder, message, context.Headers, context.Extensions)
        };

        static Task Validate<TOptions>(OutgoingMessage<object, TOptions> message, IBuilder builder)
            where TOptions : ExtendableOptions =>
            ValidateWithTypeRedirect(message.Message, message.Options, builder);

        tasks.AddRange(context.PublishedMessages.Select(_ => Validate(_, builder)));
        tasks.AddRange(context.SentMessages.Select(_ => Validate(_, builder)));
        tasks.AddRange(context.RepliedMessages.Select(_ => Validate(_, builder)));
        tasks.AddRange(context.TimeoutMessages.Select(_ => Validate(_, builder)));

        return Task.WhenAll(tasks);
    }

    internal static Task ValidateWithTypeRedirect<TOptions>(object instance, TOptions options, IBuilder builder)
        where TOptions : ExtendableOptions =>
        validator.ValidateWithTypeRedirect(instance.GetType(), builder, instance, options.GetHeaders(), options.GetExtensions());

    internal static Task InnerValidate<TMessage>(TMessage instance, IReadOnlyDictionary<string, string> headers, ContextBag contextBag, IBuilder builder)
        where TMessage : class
        => validator.Validate(instance.GetType(), builder, instance, headers, contextBag);
}