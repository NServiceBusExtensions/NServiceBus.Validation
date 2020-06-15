using FluentValidation;
using Xunit;

public class AsyncValidatorCheckerTests
{
    [Fact]
    public void IsAsync()
    {
        Assert.True(AsyncValidatorChecker.IsAsync(new AsyncMessageValidator(), new ValidationContext(new MessageWithAsyncValidator())));
        Assert.False(AsyncValidatorChecker.IsAsync(new SyncMessageValidator(), new ValidationContext(new MessageWithValidator())));
    }
}