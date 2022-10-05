using FluentValidation;

delegate bool TryGetValidators(Type messageType, IServiceProvider provider, out IEnumerable<IValidator> validators);