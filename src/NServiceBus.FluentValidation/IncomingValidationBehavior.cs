using NServiceBus.Pipeline;

class IncomingValidationBehavior :
    Behavior<IIncomingLogicalMessageContext>
{
    MessageValidator validator;

    public IncomingValidationBehavior(MessageValidator validator) =>
        this.validator = validator;

    public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var message = context.Message;
        await validator.ValidateWithTypeRedirect(message.MessageType, context.Builder.Build<IServiceProvider>(), message.Instance, context.Headers, context.Extensions);
        await next();
    }
}