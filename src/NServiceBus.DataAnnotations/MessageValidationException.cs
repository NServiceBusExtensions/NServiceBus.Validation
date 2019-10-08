using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NServiceBus.DataAnnotations
{
    public class MessageValidationException :
        Exception
    {
        public Type MessageType { get; }

        public MessageValidationException(Type messageType, IReadOnlyList<ValidationResult> errors)
        {
            Guard.AgainstNull(messageType, nameof(messageType));
            Guard.AgainstNull(errors, nameof(errors));
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
                    builder.AppendLine($" * {error.ErrorMessage}");
                }

                return builder.ToString();
            }
        }

        public IReadOnlyList<ValidationResult> Errors { get; }
    }
}