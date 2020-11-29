using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.FluentValidation;
using VerifyXunit;
using Xunit;

[UsesVerify]
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
    public async Task With_uow_validator()
    {
        MessageWithValidator message = new();
        Assert.NotNull(await Send(message, ValidatorLifecycle.UnitOfWork));
    }

    [Fact]
    public async Task With_validator_invalid()
    {
        MessageWithValidator message = new();
        Assert.NotNull(await Send(message));
    }

    [Fact]
    public async Task With_async_validator_valid()
    {
        MessageWithAsyncValidator message = new()
        {
            Content = "content"
        };
        Assert.Null(await Send(message));
    }

    [Fact]
    public async Task With_async_validator_invalid()
    {
        MessageWithAsyncValidator message = new();
        Assert.NotNull(await Send(message));
    }

    [Fact]
    public async Task Exception_message_and_errors()
    {
        MessageWithValidator message = new();
        var exception = await Send(message);
        await Verifier.Verify(exception);
    }

    static async Task<MessageValidationException> Send(object message, ValidatorLifecycle lifecycle = ValidatorLifecycle.Endpoint, [CallerMemberName] string key = "")
    {
        EndpointConfiguration configuration = new("FluentValidationIncoming" + key);
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        configuration.DisableFeature<TimeoutManager>();
        configuration.DisableFeature<Sagas>();

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
        var validation = configuration.UseFluentValidation(lifecycle, outgoing: false);
        validation.AddValidatorsFromAssemblyContaining<MessageWithNoValidator>();

        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(message);
        if (!resetEvent.WaitOne(TimeSpan.FromSeconds(10)))
        {
            throw new("No Set received.");
        }

        return exception;
    }
}