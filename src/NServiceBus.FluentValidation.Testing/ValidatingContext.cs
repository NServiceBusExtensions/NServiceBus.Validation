using System.Globalization;

namespace NServiceBus.Testing;

public class ValidatingContext<TMessage> :
    TestableMessageHandlerContext
    where TMessage : class
{
    TMessage message;
    bool hasRun;

    public ValidatingContext(TMessage message)
    {
        this.message = message;
        var value = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss:ffffff Z", CultureInfo.InvariantCulture);
        MessageHeaders.Add(NServiceBus.Headers.TimeSent, value);
        Headers.Add(NServiceBus.Headers.TimeSent, value);
    }

    public async Task Run(IHandleMessages<TMessage> handler)
    {
        hasRun = true;
        await TestContextValidator.InnerValidate(message, Headers, Extensions, Builder);
        await handler.Handle(message, this);
        AddDataIfSaga(handler);
    }

    public async Task Run(IHandleTimeouts<TMessage> handler)
    {
        hasRun = true;
        await TestContextValidator.InnerValidate(message, Headers, Extensions, Builder);
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
        await TestContextValidator.ValidateWithTypeRedirect(message, options, Builder);
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
        await TestContextValidator.ValidateWithTypeRedirect(message, options, Builder);
        await base.Reply(message, options);
    }

    public override async Task Publish(object message, PublishOptions options)
    {
        ValidateHasRun();
        await TestContextValidator.ValidateWithTypeRedirect(message, options, Builder);
        await base.Publish(message, options);
    }
}