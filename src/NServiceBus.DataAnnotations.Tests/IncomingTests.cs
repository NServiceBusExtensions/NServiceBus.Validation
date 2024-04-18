public class IncomingTests
{
    [Test]
    public async Task With_no_validator()
    {
        var message = new MessageWithNoValidator();
        Null(await Send(message));
    }

    [Test]
    public async Task With_validator_valid()
    {
        var message = new MessageWithValidator
        {
            Content = "content"
        };
        Null(await Send(message));
    }

    [Test]
    public async Task With_validator_invalid()
    {
        var message = new MessageWithValidator();
        NotNull(await Send(message));
    }

    static async Task<MessageValidationException> Send(object message, [CallerMemberName] string key = "")
    {
        var services = new ServiceCollection();

        var configuration = new EndpointConfiguration("DataAnnotationsIncoming" + key);
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        configuration.AssemblyScanner()
            .ExcludeAssemblies("xunit.runner.utility.netcoreapp10.dll");
        configuration.UseSerialization<SystemJsonSerializer>();

        using var resetEvent = new ManualResetEvent(false);
        configuration.RegisterComponents(_ => _.AddSingleton(resetEvent));
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

        var endpointProvider = EndpointWithExternallyManagedContainer
            .Create(configuration, services);

        await using var provider = services.BuildServiceProvider();
        var endpoint = await endpointProvider.Start(provider);
        await endpoint.SendLocal(message);
        if (!resetEvent.WaitOne(TimeSpan.FromSeconds(10)))
        {
            throw new("No Set received.");
        }

        await endpoint.Stop();

        return exception;
    }
}