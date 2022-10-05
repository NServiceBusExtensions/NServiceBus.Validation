﻿using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.FluentValidation;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus;

public class FluentValidationConfig
{
    IServiceCollection services;
    ValidatorLifecycle lifecycle;
    internal MessageValidator MessageValidator;

    internal FluentValidationConfig(
        IServiceCollection services,
        ValidatorLifecycle lifecycle,
        Func<Type, IValidator?>? fallback)
    {
        this.services = services;
        this.lifecycle = lifecycle;

        MessageValidator = new(GetValidatorCache(fallback));
    }

    TryGetValidators GetValidatorCache(Func<Type, IValidator?>? fallback)
    {
        if (lifecycle == ValidatorLifecycle.Endpoint)
        {
            return new EndpointValidatorTypeCache(fallback).TryGetValidators;
        }

        return new UnitOfWorkValidatorTypeCache(fallback).TryGetValidators;
    }

    public void AddValidatorsFromAssemblyContaining<T>(
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssemblyContaining(typeof(T), throwForNonPublicValidators, throwForNoValidatorsFound);

    public void AddValidatorsFromAssemblyContaining(
        Type type, bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidatorsFromAssembly(type.Assembly, throwForNonPublicValidators, throwForNoValidatorsFound);

    public void AddValidatorsFromAssembly(
        Assembly assembly,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true)
    {
        var results = ValidationFinder.FindValidatorsInAssembly(assembly, throwForNonPublicValidators, throwForNoValidatorsFound);
        AddValidators(results);
    }

    public void AddValidators(IEnumerable<Result> results)
    {
        switch (lifecycle)
        {
            case ValidatorLifecycle.Endpoint:
                foreach (var result in results)
                {
                    services.AddSingleton(result.InterfaceType, result.ValidatorType);
                }

                break;
            case ValidatorLifecycle.UnitOfWork:
                foreach (var result in results)
                {
                    services.AddScoped(result.InterfaceType, result.ValidatorType);
                }

                break;
        }
    }

    /// <summary>
    /// Register all assemblies matching *.Messages.dll that exist in AppDomain.CurrentDomain.BaseDirectory.
    /// </summary>
    public void AddValidatorsFromMessagesSuffixedAssemblies(
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        AddValidators(ValidationFinder.FindValidatorsInMessagesSuffixedAssemblies(throwForNonPublicValidators, throwForNoValidatorsFound));
}