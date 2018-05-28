using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

class ValidationBehavior : Behavior<IIncomingLogicalMessageContext>
{
    public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var message = context.Message.Instance;
        var validationContext = new ValidationContext(message, new BuilderWrapper(context.Builder),
            items: new Dictionary<object, object> {{"MessageContext", context}});
        Validator.ValidateObject(message, validationContext);
        return next();
    }
}