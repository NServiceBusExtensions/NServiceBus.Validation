using System;
using System.Globalization;
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

    public class ValidatingContext<TMessage> :
        TestableMessageHandlerContext
    {
        TMessage message;

        public ValidatingContext(TMessage message)
        {
            Guard.AgainstNull(message, nameof(message));
            this.message = message;
            var value = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss:ffffff Z", CultureInfo.InvariantCulture);
            MessageHeaders.Add(NServiceBus.Headers.TimeSent, value);
            Headers.Add(NServiceBus.Headers.TimeSent, value);
        }

        public async Task Run(IHandleMessages<TMessage> handler)
        {
            Guard.AgainstNull(handler, nameof(handler));
            await TestContextValidator.Validate(message, Headers, Extensions);
            await handler.Handle(message, this);
        }

        public async Task Run(IHandleTimeouts<TMessage> handler)
        {
            Guard.AgainstNull(handler, nameof(handler));
            await TestContextValidator.Validate(message, Headers, Extensions);
            await handler.Timeout(message, this);
        }

        public override async Task Send(object message, SendOptions options)
        {
            await TestContextValidator.Validate(message, options);
            await base.Send(message, options);
        }

        public override async Task Reply(object message, ReplyOptions options)
        {
            await TestContextValidator.Validate(message, options);
            await base.Reply(message, options);
        }

        public override async Task Publish(object message, PublishOptions options)
        {
            await TestContextValidator.Validate(message, options);
            await base.Publish(message, options);
        }
    }
}