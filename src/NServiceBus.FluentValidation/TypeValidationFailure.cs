namespace NServiceBus.FluentValidation;

public class TypeValidationFailure(Type validatorType, ValidationFailure failure)
{
    /// <summary>
    /// The <see cref="Type"/> of the <see cref="IValidator"/> that cause the <see cref="Failure"/>.
    /// </summary>
    public Type ValidatorType { get; } = validatorType;

    public ValidationFailure Failure { get; } = failure;
}