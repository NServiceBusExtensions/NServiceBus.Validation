using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.FluentValidation;
using VerifyXunit;
using Xunit;

[UsesVerify]
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
    public Task With_uow_validator()
    {
        MessageWithValidator message = new();
        return Verifier.ThrowsTask(() => Send(message, ValidatorLifecycle.UnitOfWork));
    }

    [Fact]
    public Task With_validator_invalid()
    {
        MessageWithValidator message = new();
        return Verifier.ThrowsTask(() => Send(message));
    }

    [Fact]
    public Task With_async_validator_valid()
    {
        MessageWithAsyncValidator message = new()
        {
            Content = "content"
        };
        return Send(message);
    }

    [Fact]
    public Task With_async_validator_invalid()
    {
        MessageWithAsyncValidator message = new();
        return Assert.ThrowsAsync<MessageValidationException>(() => Send(message));
    }

    static async Task Send(
        object message,
        ValidatorLifecycle lifecycle = ValidatorLifecycle.Endpoint,
        [CallerMemberName] string key = "")
    {
        EndpointConfiguration configuration = new("FluentValidationOutgoing" + key);
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        configuration.DisableFeature<TimeoutManager>();
        configuration.DisableFeature<Sagas>();

        var validation = configuration.UseFluentValidation(lifecycle, incoming: false);
        validation.AddValidatorsFromAssemblyContaining<MessageWithNoValidator>();

        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(message);
    }
}