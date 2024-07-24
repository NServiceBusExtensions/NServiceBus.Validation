using VerifyTests.NServiceBus;

namespace NServiceBus.FluentValidation.Testing;

public class ValidatingContext<TMessage>(
    TMessage message,
    IEnumerable<KeyValuePair<string, string>>? headers = null,
    IServiceProvider? provider = null)
    :
        RecordingHandlerContext(headers)
    where TMessage : class
{
    bool hasRun;

    public async Task Run(IHandleMessages<TMessage> handler)
    {
        hasRun = true;
        await TestContextValidator.InnerValidate(message, MessageHeaders, Extensions, provider);
        await handler.Handle(message, this);
        AddDataIfSaga(handler);
    }

    public async Task Run(IHandleTimeouts<TMessage> handler)
    {
        hasRun = true;
        await TestContextValidator.InnerValidate(message, MessageHeaders, Extensions, provider);
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

    public IContainSagaData? SagaData { get; private set; }



    public override async Task Send(object message, SendOptions options)
    {
        ValidateHasRun();
        await TestContextValidator.ValidateWithTypeRedirect(message, options, provider);
        await base.Send(message, options);
    }

    void ValidateHasRun()
    {
        if (!hasRun)
        {
            throw new("ValidatingContext should be executed via `validatingContext.Run(handler)`, not `handler.Handle(message, handlerContext)`.");
        }
    }

    public override async Task Reply(object message, ReplyOptions options)
    {
        ValidateHasRun();
        await TestContextValidator.ValidateWithTypeRedirect(message, options, provider);
        await base.Reply(message, options);
    }

    public override async Task Publish(object message, PublishOptions options)
    {
        ValidateHasRun();
        await TestContextValidator.ValidateWithTypeRedirect(message, options, provider);
        await base.Publish(message, options);
    }
}