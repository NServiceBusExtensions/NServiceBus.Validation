using NServiceBus;
using NServiceBus.DataAnnotations;
using NServiceBus.Features;
using Xunit;

public class OutgoingTests
{
    [Fact]
    public Task With_no_validator()
    {
        MessageWithNoValidator message = new();
        return Send(message);
    }

    [Fact]
    public Task With_validator_valid()
    {
        MessageWithValidator message = new()
        {
            Content = "content"
        };
        return Send(message);
    }

    [Fact]
    public Task With_validator_invalid()
    {
        MessageWithValidator message = new();
        return Assert.ThrowsAsync<MessageValidationException>(() => Send(message));
    }

    static async Task Send(object message, [CallerMemberName] string key = "")
    {
        EndpointConfiguration configuration = new("DataAnnotationsOutgoing" + key);
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        configuration.DisableFeature<TimeoutManager>();

        configuration.UseDataAnnotationsValidation(incoming: false);

        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(message);
    }
}