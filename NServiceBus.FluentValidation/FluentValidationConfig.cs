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
                    components.ConfigureComponent(result.ValidatorType, DependencyLifecycle.InstancePerCall);
                }
            });
        }
    }
}