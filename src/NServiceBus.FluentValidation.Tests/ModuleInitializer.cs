﻿using NServiceBus.FluentValidation.Testing;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        TestContextValidator.AddValidatorsFromAssemblyContaining<MyMessage>(throwForNonPublicValidators: false);
        VerifierSettings.IgnoreStackTrace();
    }
}