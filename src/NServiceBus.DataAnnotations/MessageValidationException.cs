namespace NServiceBus.DataAnnotations;

public class MessageValidationException(object target, IReadOnlyList<ValidationResult> errors) :
    Exception
{
    public object Target { get; } = target;
    public Type MessageType { get; } = target.GetType();

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

    public override string? StackTrace => null;

    public IReadOnlyList<ValidationResult> Errors { get; } = errors;
}