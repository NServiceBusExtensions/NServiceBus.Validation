using NServiceBus.Pipeline;
using System.ComponentModel.DataAnnotations;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to DataAnnotations.
    /// </summary>
    public static class DataAnnotationsExtensions
    {
        public static IIncomingLogicalMessageContext MessageContext(this ValidationContext validationContext)
        {
            Guard.AgainstNull(validationContext, nameof(validationContext));
            return (IIncomingLogicalMessageContext) validationContext.Items["MessageContext"];
        }
    }
}