namespace NServiceBus.FluentValidation.Testing;

public class ValidatingContext<TMessage> :
        ValidatingContext
    where TMessage : notnull
{
    bool hasRun;
    TMessage message;

    public ValidatingContext(TMessage message,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        IServiceProvider? provider = null)
        : base(headers, provider) => this.message = message;

    public async Task Run(IHandleMessages<TMessage> handler)
    {
        hasRun = true;
        await TestContextValidator.InnerValidate(message, MessageHeaders, Extensions, Provider);
        await handler.Handle(message, this);
        AddDataIfSaga(handler);
    }

    public async Task Run(IHandleTimeouts<TMessage> handler)
    {
        hasRun = true;
        await TestContextValidator.InnerValidate(message, MessageHeaders, Extensions, Provider);
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
}