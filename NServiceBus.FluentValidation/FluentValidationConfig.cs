using System;
using System.Reflection;
using FluentValidation;

namespace NServiceBus
{
    public class FluentValidationConfig
    {
        EndpointConfiguration endpointConfiguration;

        internal FluentValidationConfig(EndpointConfiguration endpointConfiguration)
        {
            this.endpointConfiguration = endpointConfiguration;
        }

        public void RegisterValidatorsFromAssemblyContaining<T>()
        {
            RegisterValidatorsFromAssemblyContaining(typeof(T));
        }

        public void RegisterValidatorsFromAssemblyContaining(Type type)
        {
            RegisterValidatorsFromAssembly(type.GetTypeInfo().Assembly);
        }

        public void RegisterValidatorsFromAssembly(Assembly assembly)
        {
            endpointConfiguration.RegisterComponents(components =>
            {
                foreach (var result in AssemblyScanner.FindValidatorsInAssembly(assembly))
                {
                    components.ConfigureComponent(result.ValidatorType, DependencyLifecycle.InstancePerCall);
                }
            });
        }
    }
}