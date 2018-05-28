using FluentValidation;
using NServiceBus.Pipeline;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to FluentValidation.
    /// </summary>
    public static class FluentValidationExtensions
    {
        public static IIncomingLogicalMessageContext MessageContext(this ValidationContext validationContext)
        {
            Guard.AgainstNull(validationContext, nameof(validationContext));
            return (IIncomingLogicalMessageContext) validationContext.RootContextData["MessageContext"];
        }
    }
}