namespace NServiceBus.FluentValidation.Testing;

public static class ValidatingContext
{
    public static ValidatingContext<TMessage> Build<TMessage>(
        TMessage message,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        IServiceProvider? provider = null)
        where TMessage : notnull =>
        new(message, headers, provider);

    public static async Task<ValidatingContext<TMessage>> Run<TMessage>(
        IHandleMessages<TMessage> handler,
        TMessage message,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        IServiceProvider? provider = null)
        where TMessage : notnull
    {
        var context = Build(message, headers, provider);
        await context.Run(handler);
        return context;
    }

    public static async Task<ValidatingContext<TMessage>> Run<TMessage>(
        IHandleTimeouts<TMessage> handler, TMessage message,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        IServiceProvider? provider = null)
        where TMessage : notnull
    {
        var context = Build(message, headers, provider);
        await context.Run(handler);
        return context;
    }
}