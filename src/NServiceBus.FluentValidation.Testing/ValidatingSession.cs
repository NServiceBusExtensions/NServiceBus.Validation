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