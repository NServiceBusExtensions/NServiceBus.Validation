using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentValidation;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus.FluentValidation
{
    public static class ValidationFinder
    {
        public static IEnumerable<Result> FindValidatorsInAssemblyContaining<T>(bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true)
        {
            return FindValidatorsInAssemblyContaining(typeof(T), throwForNonPublicValidators, throwForNoValidatorsFound);
        }

        public static IEnumerable<Result> FindValidatorsInAssemblyContaining(Type type, bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true)
        {
            Guard.AgainstNull(type, nameof(type));
            return FindValidatorsInAssembly(type.GetTypeInfo().Assembly, throwForNonPublicValidators, throwForNoValidatorsFound);
        }

        public static IEnumerable<Result> FindValidatorsInAssembly(Assembly assembly, bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true)
        {
            Guard.AgainstNull(assembly, nameof(assembly));
            var assemblyName = assembly.GetName().Name;
            if (throwForNonPublicValidators)
            {
                var openGenericType = typeof(IValidator<>);
                var nonPublicValidators = assembly
                    .GetTypes()
                    .Where(
                        type =>
                            !type.IsPublic &&
                            !type.IsNestedPublic &&
                            !type.IsAbstract &&
                            !type.IsGenericTypeDefinition &&
                            type.GetInterfaces()
                                .Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
                    )
                    .ToList();
                if (nonPublicValidators.Any())
                {
                    throw new Exception($"Found some non public validators were found in {assemblyName}:{Environment.NewLine}{string.Join(Environment.NewLine, nonPublicValidators.Select(x => x.FullName))}.");
                }
            }

            var results = AssemblyScanner.FindValidatorsInAssembly(assembly).ToList();
            if (throwForNoValidatorsFound && !results.Any())
            {
                throw new Exception($"No validators were found in {assemblyName}.");
            }

            return results;
        }

        /// <summary>
        /// Find all  assemblies matching *.Messages.dll that exist in AppDomain.CurrentDomain.BaseDirectory.
        /// </summary>
        public static IEnumerable<Result> FindValidatorsInMessagesSuffixedAssemblies()
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var messageAssemblies = Directory.EnumerateFiles(directory, "*.Messages.dll").ToList();
            if (!messageAssemblies.Any())
            {
                throw new Exception($"Could not find any assemblies matching *.Messages.dll. Directory: {directory}");
            }

            return messageAssemblies.SelectMany(x => FindValidatorsInAssembly(Assembly.LoadFrom(x)));
        }
    }
}