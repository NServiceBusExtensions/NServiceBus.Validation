using FluentValidation;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to control message validation with FluentValidation.
    /// </summary>
    public static class FluentValidationConfigurationExtensions
    {
        public static FluentValidationConfig UseFluentValidation(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(endpointConfiguration, nameof(endpointConfiguration));
            var recoverability = endpointConfiguration.Recoverability();
            recoverability.AddUnrecoverableException<ValidationException>();
            var config = new FluentValidationConfig(endpointConfiguration);
            var pipeline = endpointConfiguration.Pipeline;
            pipeline.Register(new ValidationStep(config));
            return config;
        }
    }
}