class IncomingValidationStep(MessageValidator validator) :
    RegisterStep(
        stepId: "IncomingFluentValidation",
        behavior: typeof(IncomingValidationBehavior),
        description: "Validates incoming messages using FluentValidation",
        factoryMethod: _ => new IncomingValidationBehavior(validator));