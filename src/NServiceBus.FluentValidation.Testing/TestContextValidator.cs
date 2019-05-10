using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NServiceBus.Extensibility;
using NServiceBus.FluentValidation;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus.Testing
{
    public static class TestContextValidator
    {
        static List<Result> validatorScanResults;
        static MessageValidator validator;

        static TestContextValidator()
        {
            validatorScanResults = new List<Result>();
            var typeCache = new TestingValidatorTypeCache(validatorScanResults);
            validator = new MessageValidator(typeCache);
        }

        public static void AddValidatorsFromAssemblyContaining<T>(bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true)
        {
            AddValidatorsFromAssemblyContaining(typeof(T), throwForNonPublicValidators, throwForNoValidatorsFound);
        }

        public static void AddValidatorsFromAssemblyContaining(Type type, bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true)
        {
            AddValidatorsFromAssembly(type.GetTypeInfo().Assembly, throwForNonPublicValidators, throwForNoValidatorsFound);
        }

        public static void AddValidatorsFromAssembly(Assembly assembly, bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true)
        {
            var results = ValidationFinder.FindValidatorsInAssembly(assembly, throwForNonPublicValidators, throwForNoValidatorsFound);
            AddValidators(results);
        }

        public static void AddValidators(IEnumerable<Result> results)
        {
            Guard.AgainstNull(results, nameof(results));
            validatorScanResults.AddRange(results);
        }

        /// <summary>
        /// Register all assemblies matching *.Messages.dll that exist in AppDomain.CurrentDomain.BaseDirectory.
        /// </summary>
        public static void AddValidatorsFromMessagesSuffixedAssemblies()
        {
            AddValidators(ValidationFinder.FindValidatorsInMessagesSuffixedAssemblies());
        }

        public static Task Validate<TMessage>(this TestableMessageHandlerContext context, TMessage message)
        {
            var builder = context.Builder;
            var tasks = new List<Task>
            {
                validator.Validate(message.GetType(), builder, message, context.Headers, context.Extensions)
            };

            tasks.AddRange(context.PublishedMessages.Select(Validate));
            tasks.AddRange(context.SentMessages.Select(Validate));
            tasks.AddRange(context.RepliedMessages.Select(Validate));

            return Task.WhenAll(tasks);
        }

        internal static Task Validate<T>(OutgoingMessage<object, T> message)
            where T : ExtendableOptions
        {
            var instance = message.Message;
            var options = message.Options;
            return Validate(instance, options);
        }

        internal static Task Validate<T>(object instance, T options)
            where T : ExtendableOptions
        {
            return validator.Validate(instance.GetType(), null, instance, options.GetHeaders(), options.GetExtensions());
        }

        internal static Task Validate(object instance, IReadOnlyDictionary<string, string> headers, ContextBag contextBag)
        {
            return validator.Validate(instance.GetType(), null, instance, headers, contextBag);
        }
    }
}