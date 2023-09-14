using NServiceBus.Pipeline;

class IncomingValidationBehavior(MessageValidator validator) :
    Behavior<IIncomingLogicalMessageContext>
{
    public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var message = context.Message;

        await validator.ValidateWithTypeRedirect(message.MessageType, context.Builder, message.Instance, context.Headers, context.Extensions);
        await next();
    }
}