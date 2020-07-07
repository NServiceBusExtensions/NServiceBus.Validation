using FluentValidation;
using Xunit;

public class AsyncValidatorCheckerTests
{
    [Fact]
    public void IsAsync()
    {
        var asyncMessageValidator = new AsyncMessageValidator();
        Assert.True(asyncMessageValidator.IsAsync(new ValidationContext<MessageWithAsyncValidator>(new MessageWithAsyncValidator())));

        var syncMessageValidator = new SyncMessageValidator();
        Assert.False(syncMessageValidator.IsAsync(new ValidationContext<MessageWithValidator>(new MessageWithValidator())));
    }
}