using NServiceBus.FluentValidation;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to control message validation with FluentValidation.
    /// </summary>
    public static class FluentValidationConfigurationExtensions
    {
        public static FluentValidationConfig UseFluentValidation(
            this EndpointConfiguration endpoint,
            ValidatorLifecycle lifecycle = ValidatorLifecycle.Endpoint,
            bool incoming = true,
            bool outgoing = true)
        {
            var recoverability = endpoint.Recoverability();
            recoverability.AddUnrecoverableException<MessageValidationException>();
            FluentValidationConfig config = new(endpoint, lifecycle);
            var pipeline = endpoint.Pipeline;
            if (incoming)
            {
                pipeline.Register(new IncomingValidationStep(config));
            }
            if (outgoing)
            {
                pipeline.Register(new OutgoingValidationStep(config));
            }
            return config;
        }
    }
}