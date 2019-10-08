using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NServiceBus.Extensibility;
using NServiceBus.FluentValidation;
using NServiceBus.ObjectBuilder;
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
            where TMessage : class
        {
            var builder = context.Builder;
            var tasks = new List<Task>
            {
                validator.Validate(message.GetType(), builder, message, context.Headers, context.Extensions)
            };

            tasks.AddRange(context.PublishedMessages.Select(publishedMessage => Validate(publishedMessage, builder)));
            tasks.AddRange(context.SentMessages.Select(sentMessage => Validate(sentMessage, builder)));
            tasks.AddRange(context.RepliedMessages.Select(repliedMessage => Validate(repliedMessage, builder)));
            tasks.AddRange(context.TimeoutMessages.Select(timeoutMessage => Validate(timeoutMessage, builder)));

            return Task.WhenAll(tasks);
        }

        static Task Validate<T>(OutgoingMessage<object, T> message, IBuilder builder)
            where T : ExtendableOptions
        {
            var instance = message.Message;
            var options = message.Options;
            return Validate(instance, options, builder);
        }

        internal static Task Validate<T>(object instance, T options, IBuilder builder)
            where T : ExtendableOptions
        {
            return validator.Validate(instance.GetType(), builder, instance, options.GetHeaders(), options.GetExtensions());
        }

        internal static Task Validate(object instance, IReadOnlyDictionary<string, string> headers, ContextBag contextBag, IBuilder builder)
        {
            return validator.Validate(instance.GetType(), builder, instance, headers, contextBag);
        }
    }
}