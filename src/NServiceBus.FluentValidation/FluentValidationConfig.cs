using System;
using System.Collections.Generic;
using System.Reflection;
using NServiceBus.FluentValidation;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus
{
    public class FluentValidationConfig
    {
        EndpointConfiguration endpoint;
        DependencyLifecycle dependencyLifecycle;
        internal MessageValidator MessageValidator;

        internal FluentValidationConfig(EndpointConfiguration endpoint, ValidatorLifecycle validatorLifecycle)
        {
            this.endpoint = endpoint;

            if (validatorLifecycle == ValidatorLifecycle.Endpoint)
            {
                dependencyLifecycle = DependencyLifecycle.SingleInstance;
            }
            else
            {
                dependencyLifecycle = DependencyLifecycle.InstancePerCall;
            }

            var validatorTypeCache = GetValidatorTypeCache();
            MessageValidator = new(validatorTypeCache);
        }

        TryGetValidators GetValidatorTypeCache()
        {
            if (dependencyLifecycle == DependencyLifecycle.SingleInstance)
            {
                return new EndpointValidatorTypeCache().TryGetValidators;
            }

            return new UnitOfWorkValidatorTypeCache().TryGetValidators;
        }

        public void AddValidatorsFromAssemblyContaining<T>(bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true)
        {
            AddValidatorsFromAssemblyContaining(typeof(T), throwForNonPublicValidators, throwForNoValidatorsFound);
        }

        public void AddValidatorsFromAssemblyContaining(Type type, bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true)
        {
            AddValidatorsFromAssembly(type.GetTypeInfo().Assembly, throwForNonPublicValidators, throwForNoValidatorsFound);
        }

        public void AddValidatorsFromAssembly(Assembly assembly, bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true)
        {
            var results = ValidationFinder.FindValidatorsInAssembly(assembly, throwForNonPublicValidators, throwForNoValidatorsFound);
            AddValidators(results);
        }

        public void AddValidators(IEnumerable<Result> results)
        {
            endpoint.RegisterComponents(components =>
            {
                foreach (var result in results)
                {
                    components.ConfigureComponent(result.ValidatorType, dependencyLifecycle);
                }
            });
        }

        /// <summary>
        /// Register all assemblies matching *.Messages.dll that exist in AppDomain.CurrentDomain.BaseDirectory.
        /// </summary>
        public void AddValidatorsFromMessagesSuffixedAssemblies()
        {
            AddValidators(ValidationFinder.FindValidatorsInMessagesSuffixedAssemblies());
        }
    }
}