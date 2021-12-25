using NServiceBus.FluentValidation;

[UsesVerify]
public class TestHelperTests
{
    [Fact]
    public Task FindHandledMessagesWithoutValidator()
    {
        var types = TestHelper.FindHandledMessagesWithoutValidator(typeof(Handler).Assembly, false);
        return Verify(types.ToList());
    }

    [Fact]
    public Task FindMessagesWithoutValidator()
    {
        var types = TestHelper.FindMessagesWithoutValidator(typeof(Handler).Assembly, false);
        return Verify(types.ToList());
    }
}