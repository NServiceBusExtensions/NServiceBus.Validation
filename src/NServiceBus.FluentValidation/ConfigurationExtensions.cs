using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.FluentValidation;

namespace NServiceBus;

/// <summary>
/// Extensions to control message validation with FluentValidation.
/// </summary>
public static class FluentValidationConfigurationExtensions
{
    public static FluentValidationConfig UseFluentValidation(
        this EndpointConfiguration endpoint,
        IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        bool incoming = true,
        bool outgoing = true,
        Func<Type, IValidator?>? fallback = null)
    {
        var recoverability = endpoint.Recoverability();
        recoverability.AddUnrecoverableException<MessageValidationException>();

        var config = new FluentValidationConfig(services, lifetime, fallback);
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