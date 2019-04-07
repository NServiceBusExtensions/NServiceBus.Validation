using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace NServiceBus.FluentValidation
{
    public class MessageValidationException :
        ValidationException
    {
        public Type MessageType { get; }

        public MessageValidationException(Type messageType, ICollection<ValidationFailure> errors) :
            base(errors)
        {
            Guard.AgainstNull(messageType, nameof(messageType));
            MessageType = messageType;
        }

        public override string ToString()
        {
            return $"Validation failed for message '{MessageType.FullName}'.{Environment.NewLine}{base.ToString()}";
        }
    }
}