using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.DataAnnotations;
using NServiceBus.Features;
using Xunit;
using Xunit.Abstractions;

public class IncomingTests :
    XunitApprovalBase
{
    [Fact]
    public async Task With_no_validator()
    {
        var message = new MessageWithNoValidator();
        Assert.Null(await Send(message));
    }

    [Fact]
    public async Task With_validator_valid()
    {
        var message = new MessageWithValidator
        {
            Content = "content"
        };
        Assert.Null(await Send(message));
    }

    [Fact]
    public async Task With_validator_invalid()
    {
        var message = new MessageWithValidator();
        Assert.NotNull(await Send(message));
    }

    static async Task<MessageValidationException> Send(object message, [CallerMemberName] string key = "")
    {
        var configuration = new EndpointConfiguration("DataAnnotationsIncoming" + key);
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        configuration.DisableFeature<TimeoutManager>();

        var resetEvent = new ManualResetEvent(false);
        configuration.RegisterComponents(components => components.RegisterSingleton(resetEvent));
        MessageValidationException exception = null!;
        var recoverability = configuration.Recoverability();
        recoverability.CustomPolicy(
            (config, context) =>
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
            throw new Exception("No Set received.");
        }

        return exception;
    }

    public IncomingTests(ITestOutputHelper output) : 
        base(output)
    {
    }
}