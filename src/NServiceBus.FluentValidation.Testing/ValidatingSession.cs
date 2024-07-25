namespace NServiceBus.FluentValidation.Testing;

public class ValidatingSession(
    IServiceProvider? provider = null)
    :
        RecordingMessageSession
{
    protected IServiceProvider? Provider { get; } = provider;

    public override async Task Send(object message, SendOptions options, Cancel cancel = default)
    {
        await TestContextValidator.ValidateWithTypeRedirect(message, options, Provider);
        await base.Send(message, options, cancel);
    }

    public override async Task Publish(object message, PublishOptions options, Cancel cancel = default)
    {
        await TestContextValidator.ValidateWithTypeRedirect(message, options, Provider);
        await base.Publish(message, options, cancel);
    }
}