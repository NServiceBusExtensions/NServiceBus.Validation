using FluentValidation;

#region FluentValidation_message
public class TheMessage :
    IMessage
{
    public string Content { get; set; } = null!;
}

public class MyMessageValidator :
    AbstractValidator<TheMessage>
{
    public MyMessageValidator() =>
        RuleFor(_ => _.Content)
            .NotEmpty();
}
#endregion