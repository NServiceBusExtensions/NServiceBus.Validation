using FluentValidation;

delegate bool TryGetValidators(Type messageType, IServiceProvider builder, out IEnumerable<IValidator> validators);