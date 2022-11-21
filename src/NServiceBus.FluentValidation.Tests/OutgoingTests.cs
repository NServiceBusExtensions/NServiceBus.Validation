﻿using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.FluentValidation;

[UsesVerify]
public class OutgoingTests
{
    [Fact]
    public Task With_no_validator()
    {
        var message = new MessageWithNoValidator();
        return Send(message);
    }

    [Fact]
    public Task With_no_validator_Fallback()
    {
        var message = new MessageWithNoValidator();
        return ThrowsTask(() => Send(message, fallback: _ => new FallbackValidator()));
    }

    class FallbackValidator : AbstractValidator<MessageWithNoValidator>
    {
        public FallbackValidator() =>
            RuleFor(_ => _.Content).NotEmpty();
    }

    [Fact]
    public Task With_validator_valid()
    {
        var message = new MessageWithValidator
        {
            Content = "content"
        };
        return Send(message);
    }

    [Fact]
    public Task With_uow_validator()
    {
        var message = new MessageWithValidator();
        return ThrowsTask(() => Send(message, ServiceLifetime.Scoped));
    }

    [Fact]
    public Task With_validator_invalid()
    {
        var message = new MessageWithValidator();
        return ThrowsTask(() => Send(message));
    }

    [Fact]
    public Task With_async_validator_valid()
    {
        var message = new MessageWithAsyncValidator
        {
            Content = "content"
        };
        return Send(message);
    }

    [Fact]
    public Task With_async_validator_invalid()
    {
        var message = new MessageWithAsyncValidator();
        return Assert.ThrowsAsync<MessageValidationException>(() => Send(message));
    }

    static async Task Send(
        object message,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        [CallerMemberName] string key = "",
        Func<Type, IValidator>? fallback = null)
    {
        var services = new ServiceCollection();
        var configuration = new EndpointConfiguration("FluentValidationOutgoing" + key);
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        configuration.DisableFeature<Sagas>();

        configuration.UseFluentValidation(lifetime, incoming: false, fallback: fallback);
        services.AddValidatorsFromAssemblyContaining<MessageWithNoValidator>(lifetime, throwForNonPublicValidators: false);

        var endpointProvider = EndpointWithExternallyManagedContainer
            .Create(configuration, services);

        await using var provider = services.BuildServiceProvider();
        var endpoint = await endpointProvider.Start(provider);
        await endpoint.SendLocal(message);
        await endpoint.Stop();
    }
}