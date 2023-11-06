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