public class OutgoingTests
{
    [Test]
    public Task With_no_validator()
    {
        var message = new MessageWithNoValidator();
        return Send(message);
    }

    [Test]
    public Task With_validator_valid()
    {
        var message = new MessageWithValidator
        {
            Content = "content"
        };
        return Send(message);
    }

    [Test]
    public void With_validator_invalid()
    {
        var message = new MessageWithValidator();
        ThrowsAsync<MessageValidationException>(() => Send(message));
    }

    static async Task Send(object message, [CallerMemberName] string key = "")
    {
        var services = new ServiceCollection();
        var configuration = new EndpointConfiguration("DataAnnotationsOutgoing" + key);
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);

        configuration.UseDataAnnotationsValidation(incoming: false);

        var endpointProvider = EndpointWithExternallyManagedContainer
            .Create(configuration, services);
        await using var provider = services.BuildServiceProvider();
        var endpoint = await endpointProvider.Start(provider);
        await endpoint.SendLocal(message);
        await endpoint.Stop();
    }
}