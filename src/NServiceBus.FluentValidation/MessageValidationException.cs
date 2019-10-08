using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceBus.FluentValidation
{
    public class MessageValidationException :
        Exception
    {
        public Type MessageType { get; }

        public MessageValidationException(Type messageType, IReadOnlyList<TypeValidationFailure> errors)
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
                    var failure = error.Failure;
                    builder.AppendLine($" * {failure.PropertyName}: {failure.ErrorMessage} (Validator: {error.ValidatorType.FullName})");
                }

                return builder.ToString();
            }
        }

        public IReadOnlyList<TypeValidationFailure> Errors { get; }
    }
}