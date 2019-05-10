using System.Threading.Tasks;

namespace NServiceBus.Testing
{
    public class ValidatingContext<TMessage> :
        TestableMessageHandlerContext
    {
        TMessage message;

        public ValidatingContext(TMessage message)
        {
            this.message = message;
        }

        public async Task Run(IHandleMessages<TMessage> handler)
        {
            await TestContextValidator.Validate(message, Headers, Extensions);
            await handler.Handle(message, this);
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