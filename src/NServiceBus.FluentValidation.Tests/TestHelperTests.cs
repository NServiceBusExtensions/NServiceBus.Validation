using System.Linq;
using System.Threading.Tasks;
using NServiceBus.FluentValidation;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class TestHelperTests
{
    [Fact]
    public Task FindHandledMessagesWithoutValidator()
    {
        var types = TestHelper.FindHandledMessagesWithoutValidator(typeof(Handler).Assembly, false);
        return Verifier.Verify(types.ToList());
    }

    [Fact]
    public Task FindMessagesWithoutValidator()
    {
        var types = TestHelper.FindMessagesWithoutValidator(typeof(Handler).Assembly, false);
        return Verifier.Verify(types.ToList());
    }
}