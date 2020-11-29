using FluentValidation;
using Xunit;

public class AsyncValidatorCheckerTests
{
    [Fact]
    public void IsAsync()
    {
        AsyncMessageValidator asyncMessageValidator = new();
        Assert.True(asyncMessageValidator.IsAsync(new ValidationContext<MessageWithAsyncValidator>(new MessageWithAsyncValidator())));

        SyncMessageValidator syncMessageValidator = new();
        Assert.False(syncMessageValidator.IsAsync(new ValidationContext<MessageWithValidator>(new MessageWithValidator())));
    }
}