using System.ComponentModel.DataAnnotations;

namespace NServiceBus.DataAnnotations;

public class MessageValidationException :
    Exception
{
    public object Target { get; }
    public Type MessageType { get; }

    public MessageValidationException(object target, IReadOnlyList<ValidationResult> errors)
    {
        MessageType = target.GetType();
        Errors = errors;
        Target = target;
    }

    public override string Message
    {
        get
        {
            StringBuilder builder = new($"Validation failed for message '{MessageType.FullName}'.");
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