using FluentValidation;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class AsyncValidatorCheckerTests :
    VerifyBase
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