using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.DataAnnotations;
using NServiceBus.Features;

public class OutgoingTests
{
    [Fact]
    public Task With_no_validator()
    {
        var message = new MessageWithNoValidator();
        return Send(message);
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
    public Task With_validator_invalid()
    {
        var message = new MessageWithValidator();
        return Assert.ThrowsAsync<MessageValidationException>(() => Send(message));
    }

    static async Task Send(object message, [CallerMemberName] string key = "")
    {
        var services = new ServiceCollection();
        var configuration = new EndpointConfiguration("DataAnnotationsOutgoing" + key);
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        configuration.DisableFeature<TimeoutManager>();

        configuration.UseDataAnnotationsValidation(incoming: false);

        var endpointProvider = EndpointWithExternallyManagedServiceProvider
            .Create(configuration, services);
        using var provider = services.BuildServiceProvider();
        var endpoint = await endpointProvider.Start(provider);
        await endpoint.SendLocal(message);
    }
}