using NServiceBus.Testing;

public class TestBase
{
    static TestBase()
    {
        TestContextValidator.AddValidatorsFromAssemblyContaining<MyMessage>();
    }
}