using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

class ValidationBehavior : Behavior<IIncomingLogicalMessageContext>
{
    public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        Validate(context);
        return next();
    }

    static void Validate(IIncomingLogicalMessageContext context)
    {
        var message = context.Message.Instance;
        var validationContext = new ValidationContext(
            message,
            new BuilderWrapper(context.Builder),
            items: new Dictionary<object, object>
            {
                {"MessageContext", context}
            });

        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(message, validationContext, results, true);

        if (isValid)
        {
            return;
        }

        var errorMessage = new StringBuilder();
        var error = $"Validation failed for message {message}, with the following error/s:";
        errorMessage.AppendLine(error);

        foreach (var validationResult in results)
        {
            errorMessage.AppendLine(validationResult.ErrorMessage);
        }

        throw new ValidationException(errorMessage.ToString());
    }
}