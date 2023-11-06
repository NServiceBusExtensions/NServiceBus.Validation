using FluentValidation.Results;

public class SyncMessageValidator :
    AbstractValidator<MessageWithValidator>
{
    public SyncMessageValidator() =>
        RuleFor(_ => _.Content).NotEmpty();

    public override ValidationResult Validate(ValidationContext<MessageWithValidator> context)
    {
        Assert.NotNull(context.Headers());
        Assert.NotNull(context.ContextBag());
        return base.Validate(context);
    }
}