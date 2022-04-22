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
        await validator.Validate(message.MessageType, context.Builder, message.Instance, context.Headers, context.Extensions);
        await next();
    }
}