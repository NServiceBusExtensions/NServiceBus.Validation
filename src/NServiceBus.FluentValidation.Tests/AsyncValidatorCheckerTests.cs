using FluentValidation;
using Xunit;
using Xunit.Abstractions;

public class AsyncValidatorCheckerTests :
    XunitApprovalBase
{
    [Fact]
    public void IsAsync()
    {
        Assert.True(AsyncValidatorChecker.IsAsync(new AsyncMessageValidator(), new ValidationContext(new MessageWithAsyncValidator())));
        Assert.False(AsyncValidatorChecker.IsAsync(new SyncMessageValidator(), new ValidationContext(new MessageWithValidator())));
    }

    public AsyncValidatorCheckerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}