namespace NServiceBus
{
    /// <summary>
    /// Extensions to control message validation with DataAnnotations.
    /// </summary>
    public static class DataAnnotationsConfigurationExtensions
    {
        public static void UseDataAnnotationsValidation(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(endpointConfiguration, nameof(endpointConfiguration));
            var recoverability = endpointConfiguration.Recoverability();
            recoverability.AddUnrecoverableException<ValidationException>();
            var pipeline = endpointConfiguration.Pipeline;
            pipeline.Register(new ValidationStep());
        }
    }
}