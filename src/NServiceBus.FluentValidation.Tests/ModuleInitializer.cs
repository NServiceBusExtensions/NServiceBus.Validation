using NServiceBus.Testing;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        TestContextValidator.AddValidatorsFromAssemblyContaining<MyMessage>();
    }
}