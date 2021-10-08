using NServiceBus;
using NServiceBus.DataAnnotations;
using NServiceBus.Features;
using Xunit;

public class IncomingTests
{
    [Fact]
    public async Task With_no_validator()
    {
        MessageWithNoValidator message = new();
        Assert.Null(await Send(message));
    }

    [Fact]
    public async Task With_validator_valid()
    {
        MessageWithValidator message = new()
        {
            Content = "content"
        };
        Assert.Null(await Send(message));
    }

    [Fact]
    public async Task With_validator_invalid()
    {
        MessageWithValidator message = new();
        Assert.NotNull(await Send(message));
    }

    static async Task<MessageValidationException> Send(object message, [CallerMemberName] string key = "")
    {
        EndpointConfiguration configuration = new("DataAnnotationsIncoming" + key);
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        configuration.DisableFeature<TimeoutManager>();

        ManualResetEvent resetEvent = new(false);
        configuration.RegisterComponents(components => components.RegisterSingleton(resetEvent));
        MessageValidationException exception = null!;
        var recoverability = configuration.Recoverability();
        recoverability.CustomPolicy(
            (_, context) =>
            {
                exception = (MessageValidationException) context.Exception;
                resetEvent.Set();
                return RecoverabilityAction.MoveToError("error");
            });
        configuration.UseDataAnnotationsValidation(outgoing: false);

        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(message);
        if (!resetEvent.WaitOne(TimeSpan.FromSeconds(10)))
        {
            throw new("No Set received.");
        }

        return exception;
    }
}