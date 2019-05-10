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
        static TestingValidatorTypeCache validatorTypeCache;

        static TestContextValidator()
        {
            validatorScanResults = new List<Result>();
            validatorTypeCache = new TestingValidatorTypeCache(validatorScanResults);
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

        public static Task RunValidators<TMessage>(this TestableMessageHandlerContext context, TMessage message)
        {
            var builder = context.Builder;
            var validator = new MessageValidator(validatorTypeCache);
            var tasks = new List<Task>
            {
                validator.Validate(message.GetType(), builder, message, context.Headers, context.Extensions)
            };

            var published = context.PublishedMessages
                .Select(x => validator.Validate(x.Message.GetType(), builder, x.Message, x.Options.GetHeaders(), x.Options.GetExtensions()));
            tasks.AddRange(published);

            var sent = context.SentMessages
                .Select(x => validator.Validate(x.Message.GetType(), builder, x.Message, x.Options.GetHeaders(), x.Options.GetExtensions()));
            tasks.AddRange(sent);

            var replied = context.RepliedMessages
                .Select(x => validator.Validate(x.Message.GetType(), builder, x.Message, x.Options.GetHeaders(), x.Options.GetExtensions()));
            tasks.AddRange(replied);

            return Task.WhenAll(tasks);
        }
    }
}