using System;
using System.Reflection;
using FluentValidation;

namespace NServiceBus
{
    public class FluentValidationConfig
    {
        EndpointConfiguration endpointConfiguration;
        internal ValidatorLifecycle validatorLifecycle;
        DependencyLifecycle dependencyLifecycle;

        internal FluentValidationConfig(EndpointConfiguration endpointConfiguration, ValidatorLifecycle validatorLifecycle)
        {
            this.endpointConfiguration = endpointConfiguration;
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

        public void AddValidatorsFromAssemblyContaining<T>()
        {
            AddValidatorsFromAssemblyContaining(typeof(T));
        }

        public void AddValidatorsFromAssemblyContaining(Type type)
        {
            AddValidatorsFromAssembly(type.GetTypeInfo().Assembly);
        }

        public void AddValidatorsFromAssembly(Assembly assembly)
        {
            endpointConfiguration.RegisterComponents(components =>
            {
                foreach (var result in AssemblyScanner.FindValidatorsInAssembly(assembly))
                {
                    components.ConfigureComponent(result.ValidatorType, dependencyLifecycle);
                }
            });
        }
    }
}