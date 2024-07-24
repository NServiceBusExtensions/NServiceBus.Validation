namespace NServiceBus.Testing;

public static class ValidatingContext
{
    public static ValidatingContext<TMessage> Build<TMessage>(TMessage message, IServiceProvider? provider = null)
        where TMessage : class =>
        new(message, provider);

    public static async Task<ValidatingContext<TMessage>> Run<TMessage>(IHandleMessages<TMessage> handler, TMessage message, IServiceProvider? provider = null)
        where TMessage : class
    {
        var context = Build(message, provider);
        await context.Run(handler);
        return context;
    }

    public static async Task<ValidatingContext<TMessage>> Run<TMessage>(IHandleTimeouts<TMessage> handler, TMessage message, IServiceProvider? provider = null)
        where TMessage : class
    {
        var context = Build(message, provider);
        await context.Run(handler);
        return context;
    }
}