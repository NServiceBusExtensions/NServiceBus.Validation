using System;
using FluentValidation;
using FluentValidation.Results;

namespace NServiceBus.FluentValidation
{
    public class TypeValidationFailure
    {
        /// <summary>
        /// The <see cref="Type"/> of the <see cref="IValidator"/> that cause the <see cref="Failure"/>.
        /// </summary>
        public Type ValidatorType { get; }
        public ValidationFailure Failure { get; }

        public TypeValidationFailure(Type validatorType, ValidationFailure failure)
        {
            ValidatorType = validatorType;
            Failure = failure;
        }
    }
}