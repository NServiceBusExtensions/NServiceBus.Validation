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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() =>
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        base.GetHashCode();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string? ToString() =>
        base.ToString();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) =>
        // ReSharper disable once BaseObjectEqualsIsObjectEquals
        base.Equals(obj);
}