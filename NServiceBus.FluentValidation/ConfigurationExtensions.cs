using FluentValidation;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to control message validation with FluentValidation.
    /// </summary>
    public static class FluentValidationConfigurationExtensions
    {
        public static FluentValidationConfig UseFluentValidation(this EndpointConfiguration endpointConfiguration, ValidatorLifecycle validatorLifecycle = ValidatorLifecycle.Endpoint)
        {
            Guard.AgainstNull(endpointConfiguration, nameof(endpointConfiguration));
            var recoverability = endpointConfiguration.Recoverability();
            recoverability.AddUnrecoverableException<ValidationException>();
            var config = new FluentValidationConfig(endpointConfiguration, validatorLifecycle);
            var pipeline = endpointConfiguration.Pipeline;
            pipeline.Register(new IncomingValidationStep(config));
            return config;
        }
    }
}