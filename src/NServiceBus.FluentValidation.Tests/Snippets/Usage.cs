using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.FluentValidation;

public class Usage
{
    Usage(EndpointConfiguration endpointConfiguration, IServiceCollection serviceCollection)
    {
        #region FluentValidation

        var validationConfig = endpointConfiguration.UseFluentValidation(serviceCollection);
        validationConfig.AddValidatorsFromAssemblyContaining<TheMessage>();

        #endregion

        #region FluentValidation_disableincoming

        endpointConfiguration.UseFluentValidation(
            serviceCollection,
            incoming: false);

        #endregion

        #region FluentValidation_disableoutgoing

        endpointConfiguration.UseFluentValidation(
            serviceCollection,
            outgoing: false);

        #endregion

        // ReSharper disable once RedundantArgumentDefaultValue

        #region FluentValidation_EndpointLifecycle

        endpointConfiguration.UseFluentValidation(serviceCollection, ValidatorLifecycle.Endpoint);

        #endregion

        #region FluentValidation_UnitOfWorkLifecycle

        endpointConfiguration.UseFluentValidation(serviceCollection, ValidatorLifecycle.UnitOfWork);

        #endregion
    }

    void AddValidators(EndpointConfiguration endpointConfiguration, Assembly assembly, IServiceCollection serviceCollection)
    {
        #region FluentValidation_AddValidators

        var validationConfig = endpointConfiguration.UseFluentValidation(serviceCollection);
        validationConfig.AddValidatorsFromAssemblyContaining<MyMessage>();
        validationConfig.AddValidatorsFromAssemblyContaining(typeof(SomeOtherMessage));
        validationConfig.AddValidatorsFromAssembly(assembly);

        #endregion
    }

    void IgnoreValidatorConventions(EndpointConfiguration endpointConfiguration, Assembly assembly, IServiceCollection serviceCollection)
    {
        #region FluentValidation_IgnoreValidatorConventions

        var validationConfig = endpointConfiguration.UseFluentValidation(serviceCollection);
        validationConfig.AddValidatorsFromAssembly(assembly,
            throwForNonPublicValidators: false,
            throwForNoValidatorsFound: false);

        #endregion
    }
}