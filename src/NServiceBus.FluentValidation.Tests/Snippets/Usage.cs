using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

public class Usage
{
    Usage(EndpointConfiguration endpointConfiguration, IServiceCollection serviceCollection)
    {
        #region FluentValidation

        endpointConfiguration.UseFluentValidation();
        serviceCollection.AddValidatorsFromAssemblyContaining<TheMessage>();

        #endregion

        #region FluentValidation_disableincoming

        endpointConfiguration.UseFluentValidation(incoming: false);

        #endregion

        #region FluentValidation_disableoutgoing

        endpointConfiguration.UseFluentValidation(outgoing: false);

        #endregion

        // ReSharper disable once RedundantArgumentDefaultValue

        #region FluentValidation_Singleton

        endpointConfiguration.UseFluentValidation(ServiceLifetime.Singleton);

        #endregion

        #region FluentValidation_Scoped

        endpointConfiguration.UseFluentValidation(ServiceLifetime.Scoped);

        #endregion
    }

    void AddValidators(EndpointConfiguration endpointConfiguration, Assembly assembly, IServiceCollection serviceCollection)
    {
        #region FluentValidation_AddValidators

        endpointConfiguration.UseFluentValidation();
        serviceCollection.AddValidatorsFromAssemblyContaining<MyMessage>();
        serviceCollection.AddValidatorsFromAssemblyContaining(typeof(SomeOtherMessage));
        serviceCollection.AddValidatorsFromAssembly(assembly);

        #endregion
    }

    void IgnoreValidatorConventions(EndpointConfiguration endpointConfiguration, Assembly assembly, IServiceCollection serviceCollection)
    {
        #region FluentValidation_IgnoreValidatorConventions

        endpointConfiguration.UseFluentValidation();
        serviceCollection.AddValidatorsFromAssembly(
            assembly,
            throwForNonPublicValidators: false,
            throwForNoValidatorsFound: false);

        #endregion
    }
}