using NServiceBus.DataAnnotations;

namespace NServiceBus;

/// <summary>
/// Extensions to control message validation with DataAnnotations.
/// </summary>
public static class DataAnnotationsConfigurationExtensions
{
    public static void UseDataAnnotationsValidation(
        this EndpointConfiguration endpoint,
        bool incoming = true,
        bool outgoing = true)
    {
        var recoverability = endpoint.Recoverability();
        recoverability.AddUnrecoverableException<MessageValidationException>();
        var pipeline = endpoint.Pipeline;
        if (incoming)
        {
            pipeline.Register(new IncomingValidationStep());
        }

        if (outgoing)
        {
            pipeline.Register(new OutgoingValidationStep());
        }
    }
}