using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using NServiceBus.Extensibility;
using NServiceBus.FluentValidation;
using NServiceBus.ObjectBuilder;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus.Testing
{
    public static class TestContextValidator
    {
        static TestContextValidator()
        {
            registerMethod = typeof(FakeBuilder)
                .GetTypeInfo()
                .GetDeclaredMethods(nameof(FakeBuilder.Register))
                .Single(x => !x.GetParameters().Single().ParameterType.IsArray);
        }
        static List<Result> validatorScanResults = new List<Result>();
        static MethodInfo registerMethod;

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
        public static async Task RunValidators<TMessage>(this TestableMessageHandlerContext context, TMessage message)
        {

            var builder = context.Builder;
            foreach (var validatorScanResult in validatorScanResults)
            {
                var method = registerMethod.MakeGenericMethod(validatorScanResult.InterfaceType);
                method.Invoke(builder,)
                builder.
            }
            var validator = new MessageValidator(new TestingValidatorTypeCache());

            await validator.Validate(message.GetType(), builder, message, context.Headers, context.Extensions);

            foreach (var published in context.PublishedMessages)
            {
                await validator.Validate(published.Message.GetType(), builder, published.Message, published.Options.GetHeaders(), published.Options.GetExtensions());
            }
            foreach (var sent in context.SentMessages)
            {
                await validator.Validate(sent.Message.GetType(), builder, sent.Message, sent.Options.GetHeaders(), sent.Options.GetExtensions());
            }
            foreach (var replied in context.RepliedMessages)
            {
                await validator.Validate(replied.Message.GetType(), builder, replied.Message, replied.Options.GetHeaders(), replied.Options.GetExtensions());
            }
        }
    }
}