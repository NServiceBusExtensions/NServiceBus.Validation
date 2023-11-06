public class Usage
{
    Usage(EndpointConfiguration configuration)
    {
        #region DataAnnotations

        configuration.UseDataAnnotationsValidation();

        #endregion

        #region DataAnnotations_disableincoming

        configuration.UseDataAnnotationsValidation(incoming: false);

        #endregion

        #region DataAnnotations_disableoutgoing

        configuration.UseDataAnnotationsValidation(outgoing: false);

        #endregion
    }
}