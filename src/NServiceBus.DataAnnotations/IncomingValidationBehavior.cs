using NServiceBus.Pipeline;

class IncomingValidationBehavior :
    Behavior<IIncomingLogicalMessageContext>
{
    IServiceProvider provider;

    public IncomingValidationBehavior(IServiceProvider provider) =>
        this.provider = provider;

    public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        Validate(context);
        return next();
    }

    void Validate(IIncomingLogicalMessageContext context) =>
        MessageValidator.Validate(context.Message.Instance, provider, context.Headers, context.Extensions);
}