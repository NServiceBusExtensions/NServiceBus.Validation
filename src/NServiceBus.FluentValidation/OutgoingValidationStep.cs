class OutgoingValidationStep(MessageValidator validator) :
    RegisterStep(
        stepId: "OutgoingFluentValidation",
        behavior: typeof(OutgoingValidationBehavior),
        description: "Validates outgoing messages using FluentValidation",
        factoryMethod: _ => new OutgoingValidationBehavior(validator));