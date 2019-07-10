using System;
using System.Globalization;
using System.Threading.Tasks;

namespace NServiceBus.Testing
{
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
            AddDataIfSaga(handler);
        }

        public async Task Run(IHandleTimeouts<TMessage> handler)
        {
            Guard.AgainstNull(handler, nameof(handler));
            await TestContextValidator.Validate(message, Headers, Extensions);
            await handler.Timeout(message, this);
            AddDataIfSaga(handler);
        }

        void AddDataIfSaga(object handler)
        {
            if (handler is Saga saga)
            {
                Extensions.Set("SagaData", saga.Entity);
            }
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