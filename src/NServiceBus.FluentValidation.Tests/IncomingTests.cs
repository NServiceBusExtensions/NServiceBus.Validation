﻿using System;
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
    public async Task With_uow_validator()
    {
        var message = new MessageWithValidator();
        Assert.NotNull(await Send(message, ValidatorLifecycle.UnitOfWork));
    }

    [Fact]
    public async Task With_validator_invalid()
    {
        var message = new MessageWithValidator();
        Assert.NotNull(await Send(message));
    }

    [Fact]
    public async Task With_async_validator_valid()
    {
        var message = new MessageWithAsyncValidator
        {
            Content = "content"
        };
        Assert.Null(await Send(message));
    }

    [Fact]
    public async Task With_async_validator_invalid()
    {
        var message = new MessageWithAsyncValidator();
        Assert.NotNull(await Send(message));
    }

    [Fact]
    public async Task Exception_message_and_errors()
    {
        var message = new MessageWithValidator();
        var exception = await Send(message);
        await Verifier.Verify(exception);
    }

    static async Task<MessageValidationException> Send(object message, ValidatorLifecycle lifecycle = ValidatorLifecycle.Endpoint, [CallerMemberName] string key = "")
    {
        var configuration = new EndpointConfiguration("FluentValidationIncoming" + key);
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        configuration.DisableFeature<TimeoutManager>();
        configuration.DisableFeature<Sagas>();

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
        var validation = configuration.UseFluentValidation(lifecycle, outgoing: false);
        validation.AddValidatorsFromAssemblyContaining<MessageWithNoValidator>();

        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(message);
        if (!resetEvent.WaitOne(TimeSpan.FromSeconds(10)))
        {
            throw new Exception("No Set received.");
        }

        return exception;
    }
}