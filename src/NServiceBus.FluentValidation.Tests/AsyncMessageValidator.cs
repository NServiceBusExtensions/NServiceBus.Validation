using System.Threading.Tasks;
using FluentValidation;

public class AsyncMessageValidator :
    AbstractValidator<MessageWithAsyncValidator>
{
    public AsyncMessageValidator()
    {
        RuleFor(_ => _.Content)
            .MustAsync((s, _) => Task.FromResult(s != null));
    }
}