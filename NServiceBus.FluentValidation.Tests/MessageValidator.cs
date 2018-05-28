using FluentValidation;
using FluentValidation.Results;
using NServiceBus;
using Xunit;

public class MessageValidator : AbstractValidator<MessageWithValidator>
{
    public MessageValidator()
    {
        RuleFor(_ => _.Content).NotEmpty();
    }

    public override ValidationResult Validate(ValidationContext<MessageWithValidator> context)
    {
        var messageContext = context.MessageContext();
        Assert.NotNull(messageContext);
        return base.Validate(context);
    }
}