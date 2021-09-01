using System.Runtime.CompilerServices;
using NServiceBus.Testing;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        TestContextValidator.AddValidatorsFromAssemblyContaining<MyMessage>(throwForNonPublicValidators: false);
    }
}