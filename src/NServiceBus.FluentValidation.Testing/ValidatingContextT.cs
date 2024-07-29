namespace NServiceBus.FluentValidation.Testing;

public class ValidatingContext<TMessage>(
    TMessage message,
    IEnumerable<KeyValuePair<string, string>>? headers = null,
    IServiceProvider? provider = null)
    :
        ValidatingContext(headers, provider)
    where TMessage : notnull
{
    bool hasRun;

    public async Task Run(IHandleMessages<TMessage> handler)
    {
        hasRun = true;
        await TestContextValidator.InnerValidate(message, ((IMessageProcessingContext)this).MessageHeaders, Extensions, Provider);
        await handler.Handle(message, this);
        AddDataIfSaga(handler);
    }

    public async Task Run(IHandleTimeouts<TMessage> handler)
    {
        hasRun = true;
        await TestContextValidator.InnerValidate(message, ((IMessageProcessingContext)this).MessageHeaders, Extensions, Provider);
        await handler.Timeout(message, this);
        AddDataIfSaga(handler);
    }

    void AddDataIfSaga(object handler)
    {
        if (handler is Saga saga)
        {
            SagaData = saga.Entity;
        }
    }

    void ValidateHasRun()
    {
        if (!hasRun)
        {
            throw new("ValidatingContext should be executed via `validatingContext.Run(handler)`, not `handler.Handle(message, handlerContext)`.");
        }
    }

    public IContainSagaData? SagaData { get; private set; }

    public override Task Send(object message, SendOptions options)
    {
        ValidateHasRun();
        return base.Send(message, options);
    }

    public override Task Reply(object message, ReplyOptions options)
    {
        ValidateHasRun();
        return base.Reply(message, options);
    }

    public override Task Publish(object message, PublishOptions options)
    {
        ValidateHasRun();
        return base.Publish(message, options);
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