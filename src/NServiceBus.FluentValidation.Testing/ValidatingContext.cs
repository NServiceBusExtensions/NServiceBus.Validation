using System.Globalization;

namespace NServiceBus.Testing;

public class ValidatingContext<TMessage> :
    TestableMessageHandlerContext
    where TMessage : class
{
    TMessage message;
    IServiceProvider provider;
    bool hasRun;

    public ValidatingContext(TMessage message, IServiceProvider provider)
    {
        this.message = message;
        this.provider = provider;
        var value = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss:ffffff Z", CultureInfo.InvariantCulture);
        MessageHeaders.Add(NServiceBus.Headers.TimeSent, value);
        Headers.Add(NServiceBus.Headers.TimeSent, value);
    }

    public async Task Run(IHandleMessages<TMessage> handler)
    {
        hasRun = true;
        await TestContextValidator.InnerValidate(message, Headers, Extensions, provider);
        await handler.Handle(message, this);
        AddDataIfSaga(handler);
    }

    public async Task Run(IHandleTimeouts<TMessage> handler)
    {
        hasRun = true;
        await TestContextValidator.InnerValidate(message, Headers, Extensions, provider);
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