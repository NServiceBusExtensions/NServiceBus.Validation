using System.Threading.Tasks;

namespace NServiceBus.Testing
{
    public static class ValidatingContext
    {
        public static ValidatingContext<TMessage> Build<TMessage>(TMessage message)
        {
            Guard.AgainstNull(message, nameof(message));
            return new ValidatingContext<TMessage>(message);
        }

        public static async Task<ValidatingContext<TMessage>> Run<TMessage>(IHandleMessages<TMessage> handler, TMessage message)
        {
            Guard.AgainstNull(message, nameof(message));
            Guard.AgainstNull(handler, nameof(handler));
            var context = Build(message);
            await context.Run(handler);
            return context;
        }

        public static async Task<ValidatingContext<TMessage>> Run<TMessage>(IHandleTimeouts<TMessage> handler, TMessage message)
        {
            Guard.AgainstNull(message, nameof(message));
            Guard.AgainstNull(handler, nameof(handler));
            var context = Build(message);
            await context.Run(handler);
            return context;
        }
    }
}