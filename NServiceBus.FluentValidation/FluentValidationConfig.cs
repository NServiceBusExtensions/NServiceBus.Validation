using System;
using System.Linq;
using System.Reflection;
using FluentValidation;

namespace NServiceBus
{
    public class FluentValidationConfig
    {
        EndpointConfiguration endpoint;
        internal ValidatorLifecycle validatorLifecycle;
        DependencyLifecycle dependencyLifecycle;

        internal FluentValidationConfig(EndpointConfiguration endpoint, ValidatorLifecycle validatorLifecycle)
        {
            this.endpoint = endpoint;
            this.validatorLifecycle = validatorLifecycle;

            if (validatorLifecycle == ValidatorLifecycle.Endpoint)
            {
                dependencyLifecycle = DependencyLifecycle.SingleInstance;
            }
            else
            {
                dependencyLifecycle = DependencyLifecycle.InstancePerCall;
            }
        }

        IValidatorTypeCache GetValidatorTypeCache()
        {
            if (validatorLifecycle == ValidatorLifecycle.Endpoint)
            {
                return new EndpointValidatorTypeCache();
            }

            return new UnitOfWorkValidatorTypeCache();
        }

        internal MessageValidator BuildMessageValidator()
        {
            var validatorTypeCache = GetValidatorTypeCache();
            return new MessageValidator(validatorTypeCache);
        }

        public void AddValidatorsFromAssemblyContaining<T>(bool throwForNonPublicValidators = true)
        {
            AddValidatorsFromAssemblyContaining(typeof(T), throwForNonPublicValidators);
        }

        public void AddValidatorsFromAssemblyContaining(Type type, bool throwForNonPublicValidators = true)
        {
            AddValidatorsFromAssembly(type.GetTypeInfo().Assembly, throwForNonPublicValidators);
        }

        public void AddValidatorsFromAssembly(Assembly assembly, bool throwForNonPublicValidators = true)
        {
            if (throwForNonPublicValidators)
            {
                var openGenericType = typeof(IValidator<>);
                var nonPublicValidators = assembly
                    .GetTypes()
                    .Where(type => !type.IsPublic &&
                                   !type.IsNestedPublic &&
                                   !type.IsAbstract &&
                                   !type.IsGenericTypeDefinition &&
                                   type.GetInterfaces()
                                       .Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
                    )
                    .ToList();
                if (nonPublicValidators.Any())
                {
                    throw new Exception($"Found some non public validators were found:{Environment.NewLine}{string.Join(Environment.NewLine, nonPublicValidators.Select(x => x.FullName))}");
                }
            }

            endpoint.RegisterComponents(components =>
            {
                foreach (var result in AssemblyScanner.FindValidatorsInAssembly(assembly))
                {
                    components.ConfigureComponent(result.ValidatorType, dependencyLifecycle);
                }
            });
        }
    }
}