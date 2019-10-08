using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.Results;

namespace NServiceBus.FluentValidation
{
    public class MessageValidationException :
        Exception
    {
        public Type MessageType { get; }

        public MessageValidationException(Type messageType, IReadOnlyList<ValidationFailure> errors)
        {
            Guard.AgainstNull(messageType, nameof(messageType));
            MessageType = messageType;
            Errors = errors;
        }

        public override string Message
        {
            get
            {
                var builder = new StringBuilder($"Validation failed for message '{MessageType.FullName}'.");
                builder.AppendLine();
                foreach (var error in Errors)
                {
                    builder.AppendLine($" * {error.PropertyName}: {error.ErrorMessage}");
                }

                return builder.ToString();
            }
        }

        public IReadOnlyList<ValidationFailure> Errors { get; }
    }
}