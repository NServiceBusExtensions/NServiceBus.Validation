﻿using System.ComponentModel.DataAnnotations;
using NServiceBus.DataAnnotations;
using NServiceBus.Extensibility;
using NServiceBus.ObjectBuilder;

static class MessageValidator
{
    public static void Validate(object message, IBuilder builder, Dictionary<string, string> headers, ContextBag contextBag)
    {
        var validationContext = new ValidationContext(
            message,
            new BuilderWrapper(builder),
            items: new Dictionary<object, object?>
        {
            {
                "Headers", headers
            },
            {
                "ContextBag", contextBag
            },
        });

        var results = new List<ValidationResult>();

        if (Validator.TryValidateObject(message, validationContext, results, true))
        {
            return;
        }

        var errorMessage = new StringBuilder();
        var error = $"Validation failed for message '{message.GetType()}'.";
        errorMessage.AppendLine(error);

        foreach (var result in results)
        {
            errorMessage.AppendLine(result.ErrorMessage);
        }

        throw new MessageValidationException(message, results);
    }
}