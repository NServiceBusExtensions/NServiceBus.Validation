namespace NServiceBus
{
    /// <summary>
    /// Extensions to control message validation with DataAnnotations.
    /// </summary>
    public static class DataAnnotationsConfigurationExtensions
    {
        public static void UseDataAnnotationsValidation(this EndpointConfiguration endpoint, bool validateOutgoingMessages = false)
        {
            Guard.AgainstNull(endpoint, nameof(endpoint));
            var recoverability = endpoint.Recoverability();
            recoverability.AddUnrecoverableException<ValidationException>();
            var pipeline = endpoint.Pipeline;
            pipeline.Register(new IncomingValidationStep());
            if (validateOutgoingMessages)
            {
                pipeline.Register(new OutgoingValidationStep());
            }
        }
    }
}