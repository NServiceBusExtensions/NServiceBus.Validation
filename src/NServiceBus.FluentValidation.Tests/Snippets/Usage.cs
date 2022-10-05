using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

public class Usage
{
    Usage(EndpointConfiguration endpointConfiguration, IServiceCollection serviceCollection)
    {
        #region FluentValidation

        endpointConfiguration.UseFluentValidation(serviceCollection);
        FluentValidationConfig.AddValidatorsFromAssemblyContaining<TheMessage>(serviceCollection);

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

        #region FluentValidation_Singleton

        endpointConfiguration.UseFluentValidation(serviceCollection, ServiceLifetime.Singleton);

        #endregion

        #region FluentValidation_Scoped

        endpointConfiguration.UseFluentValidation(serviceCollection, ServiceLifetime.Scoped);

        #endregion
    }

    void AddValidators(EndpointConfiguration endpointConfiguration, Assembly assembly, IServiceCollection serviceCollection)
    {
        #region FluentValidation_AddValidators

        endpointConfiguration.UseFluentValidation(serviceCollection);
        FluentValidationConfig.AddValidatorsFromAssemblyContaining<MyMessage>(serviceCollection);
        FluentValidationConfig.AddValidatorsFromAssemblyContaining(serviceCollection,typeof(SomeOtherMessage));
        FluentValidationConfig.AddValidatorsFromAssembly(serviceCollection, assembly);

        #endregion
    }

    void IgnoreValidatorConventions(EndpointConfiguration endpointConfiguration, Assembly assembly, IServiceCollection serviceCollection)
    {
        #region FluentValidation_IgnoreValidatorConventions

        endpointConfiguration.UseFluentValidation(serviceCollection);
        FluentValidationConfig.AddValidatorsFromAssembly(
            serviceCollection,
            assembly,
            throwForNonPublicValidators: false,
            throwForNoValidatorsFound: false);

        #endregion
    }
}