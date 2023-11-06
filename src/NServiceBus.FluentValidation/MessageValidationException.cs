namespace NServiceBus.FluentValidation;

public class MessageValidationException(object target, IReadOnlyList<TypeValidationFailure> errors) :
    Exception
{
    public object Target { get; } = target;
    public Type MessageType { get; } = target.GetType();

    public string UserMessage
    {
        get
        {
            var builder = new StringBuilder($"Validation failed for '{MessageType.Name}'.");
            builder.AppendLine();
            foreach (var error in Errors)
            {
                var failure = error.Failure;
                builder.AppendLine($" * {failure.ErrorMessage}");
            }

            return builder.ToString();
        }
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
                builder.AppendLine($" * {failure.ErrorMessage} (Validator: {error.ValidatorType.FullName})");
            }

            return builder.ToString();
        }
    }

    public IReadOnlyList<TypeValidationFailure> Errors { get; } = errors;
}