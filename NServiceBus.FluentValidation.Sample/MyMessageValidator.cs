using FluentValidation;

public class MyMessageValidator : AbstractValidator<MyMessage>
{
    public MyMessageValidator()
    {
        RuleFor(_ => _.Content).NotEmpty();
    }
}