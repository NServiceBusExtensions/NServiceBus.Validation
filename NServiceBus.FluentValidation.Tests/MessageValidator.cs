using FluentValidation;

public class MessageValidator : AbstractValidator<MessageWithValidator>
{
    public MessageValidator()
    {
        RuleFor(_ => _.Content).NotEmpty();
    }
}