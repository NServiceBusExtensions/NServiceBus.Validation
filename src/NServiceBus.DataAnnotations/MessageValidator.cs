﻿using System.ComponentModel.DataAnnotations;
using NServiceBus.DataAnnotations;
using NServiceBus.Extensibility;

static class MessageValidator
{
    public static void Validate(object message, IServiceProvider builder, Dictionary<string, string> headers, ContextBag contextBag)
    {
        ValidationContext validationContext = new(
            message,
            builder,
            items: new Dictionary<object, object?>
            {
                {"Headers", headers},
                {"ContextBag", contextBag},
            });

        List<ValidationResult> results = new();

        if (Validator.TryValidateObject(message, validationContext, results, true))
        {
            return;
        }

        StringBuilder errorMessage = new();
        var error = $"Validation failed for message '{message.GetType()}'.";
        errorMessage.AppendLine(error);

        foreach (var result in results)
        {
            errorMessage.AppendLine(result.ErrorMessage);
        }

        throw new MessageValidationException(message, results);
    }
}