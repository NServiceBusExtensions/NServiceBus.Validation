namespace NServiceBus.FluentValidation.Testing;

public partial class ValidatingContext(
    IEnumerable<KeyValuePair<string, string>>? headers = null,
    IServiceProvider? provider = null)
    :
        RecordingHandlerContext(headers)
{
    protected IServiceProvider? Provider { get; } = provider;

    public override async Task Send(object message, SendOptions options)
    {
        await TestContextValidator.ValidateWithTypeRedirect(message, options, Provider);
        await base.Send(message, options);
    }

    public override async Task Reply(object message, ReplyOptions options)
    {
        await TestContextValidator.ValidateWithTypeRedirect(message, options, Provider);
        await base.Reply(message, options);
    }

    public override async Task Publish(object message, PublishOptions options)
    {
        await TestContextValidator.ValidateWithTypeRedirect(message, options, Provider);
        await base.Publish(message, options);
    }
}