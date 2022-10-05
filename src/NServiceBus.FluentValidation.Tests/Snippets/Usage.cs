using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

public class Usage
{
    Usage(EndpointConfiguration endpointConfiguration, IServiceCollection serviceCollection)
    {
        #region FluentValidation

        endpointConfiguration.UseFluentValidation(serviceCollection);
        serviceCollection.AddValidatorsFromAssemblyContaining<TheMessage>();

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
        serviceCollection.AddValidatorsFromAssemblyContaining<MyMessage>();
        serviceCollection.AddValidatorsFromAssemblyContaining(typeof(SomeOtherMessage));
        serviceCollection.AddValidatorsFromAssembly(assembly);

        #endregion
    }

    void IgnoreValidatorConventions(EndpointConfiguration endpointConfiguration, Assembly assembly, IServiceCollection serviceCollection)
    {
        #region FluentValidation_IgnoreValidatorConventions

        endpointConfiguration.UseFluentValidation(serviceCollection);
        serviceCollection.AddValidatorsFromAssembly(
            assembly,
            throwForNonPublicValidators: false,
            throwForNoValidatorsFound: false);

        #endregion
    }
}