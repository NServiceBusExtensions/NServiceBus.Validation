using FluentValidation;
using NServiceBus.FluentValidation;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus;

public class FluentValidationConfig
{
    EndpointConfiguration endpoint;
    DependencyLifecycle dependencyLifecycle;
    internal MessageValidator MessageValidator;

    internal FluentValidationConfig(EndpointConfiguration endpoint, ValidatorLifecycle validatorLifecycle, Func<Type, IValidator?>? fallback)
    {
        this.endpoint = endpoint;

        if (validatorLifecycle == ValidatorLifecycle.Endpoint)
        {
            dependencyLifecycle = DependencyLifecycle.SingleInstance;
        }
        else
        {
            dependencyLifecycle = DependencyLifecycle.InstancePerCall;
        }

        MessageValidator = new(GetValidatorCache(fallback));
    }

    TryGetValidators GetValidatorCache(Func<Type, IValidator?>? fallback)
    {
        if (dependencyLifecycle == DependencyLifecycle.SingleInstance)
        {
            return new EndpointValidatorTypeCache(fallback).TryGetValidators;
        }

        return new UnitOfWorkValidatorTypeCache(fallback).TryGetValidators;
    }

    public void AddValidatorsFromAssemblyContaining<T>(bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssemblyContaining(typeof(T), throwForNonPublicValidators, throwForNoValidatorsFound);

    public void AddValidatorsFromAssemblyContaining(Type type, bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssembly(type.Assembly, throwForNonPublicValidators, throwForNoValidatorsFound);

    public void AddValidatorsFromAssembly(Assembly assembly, bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true)
    {
        var results = ValidationFinder.FindValidatorsInAssembly(assembly, throwForNonPublicValidators, throwForNoValidatorsFound);
        AddValidators(results);
    }

    public void AddValidators(IEnumerable<Result> results) =>
        endpoint.RegisterComponents(components =>
        {
            foreach (var result in results)
            {
                components.ConfigureComponent(result.ValidatorType, dependencyLifecycle);
            }
        });

    /// <summary>
    /// Register all assemblies matching *.Messages.dll that exist in AppDomain.CurrentDomain.BaseDirectory.
    /// </summary>
    public void AddValidatorsFromMessagesSuffixedAssemblies(bool throwForNonPublicValidators = true, bool throwForNoValidatorsFound = true) =>
        AddValidators(ValidationFinder.FindValidatorsInMessagesSuffixedAssemblies(throwForNonPublicValidators, throwForNoValidatorsFound));
}