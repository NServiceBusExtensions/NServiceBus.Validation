using FluentValidation;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to control message validation with FluentValidation.
    /// </summary>
    public static class FluentValidationConfigurationExtensions
    {
        public static FluentValidationConfig UseFluentValidation(this EndpointConfiguration endpoint, ValidatorLifecycle validatorLifecycle = ValidatorLifecycle.Endpoint, bool validateOutgoingMessages = false)
        {
            Guard.AgainstNull(endpoint, nameof(endpoint));
            var recoverability = endpoint.Recoverability();
            recoverability.AddUnrecoverableException<ValidationException>();
            var config = new FluentValidationConfig(endpoint, validatorLifecycle);
            var pipeline = endpoint.Pipeline;
            pipeline.Register(new IncomingValidationStep(config));
            if (validateOutgoingMessages)
            {
                pipeline.Register(new OutgoingValidationStep(config));
            }
            return config;
        }
    }
}