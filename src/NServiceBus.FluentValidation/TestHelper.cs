namespace NServiceBus.FluentValidation;

public static class TestHelper
{
    public static IEnumerable<Type> FindMessagesWithoutValidator(params Assembly[] messageAssemblies) =>
        messageAssemblies.SelectMany(assembly => FindMessagesWithoutValidator(assembly));

    public static IEnumerable<Type> FindMessagesWithoutValidator(
        Assembly messageAssemblies,
        bool throwForNonPublicValidators = true)
    {
        var messageTypes = messageAssemblies
            .GetTypes()
            .Where(_ => _.IsMessage())
            .ToList();

        foreach (var validator in messageAssemblies
                     .GetTypes()
                     .Where(_ => _.IsValidator(throwForNonPublicValidators)))
        {
            // if a validator handles an IMessage remove that message type from the messageTypes list
            var interfaces = validator.GetInterfaces();
            var args = interfaces
                .Select(_ => _.GenericTypeArguments)
                .SelectMany(_ => _)
                .ToList();
            var messageType = args.FirstOrDefault(_ => _.IsMessage());
            if (messageType is not null)
            {
                if (messageTypes.Contains(messageType))
                {
                    messageTypes.Remove(messageType);
                }
            }
        }

        return messageTypes;
    }

    public static IEnumerable<Type> FindHandledMessagesWithoutValidator(params Assembly[] handlerAssemblies) =>
        handlerAssemblies.SelectMany(assembly => FindHandledMessagesWithoutValidator(assembly));

    public static IEnumerable<Type> FindHandledMessagesWithoutValidator(
        Assembly handlerAssembly,
        bool throwForNonPublicValidators = true)
    {
        var tracking = new List<(Type messageType, Type validatorOrHandler)>();

        foreach (var t in handlerAssembly.GetClasses())
        {
            var interfaces = t.GetInterfaces();
            foreach (var argType in AllGenericArgs(interfaces))
            {
                if (!argType.IsMessage())
                {
                    continue;
                }

                if (t.IsValidator(throwForNonPublicValidators))
                {
                    tracking.Add((messageType: argType, validatorOrHandler: typeof(IValidator)));
                }

                if (interfaces.Any(_ => _.Name.Equals(typeof(IHandleMessages<>).Name)))
                {
                    tracking.Add((messageType: argType, validatorOrHandler: typeof(IHandleMessages<>)));
                }
            }
        }

        foreach (var messageGroup in tracking.GroupBy(_ => _.messageType))
        {
            var handlerCount = messageGroup.Count(_ => _.validatorOrHandler == typeof(IHandleMessages<>));
            var validatorCount = messageGroup.Count(_ => _.validatorOrHandler == typeof(IValidator));

            if (handlerCount > 0 && validatorCount == 0)
            {
                yield return messageGroup.Key;
            }
        }
    }

    static IEnumerable<Type> AllGenericArgs(Type[] interfaces) =>
        interfaces
            .Select(_ => _.GenericTypeArguments)
            .SelectMany(_ => _);

    static IEnumerable<Type> GetClasses(this Assembly handlerAssembly) =>
        handlerAssembly
            .GetTypes()
            .Where(_ => !_.IsInterface);

    static bool IsValidator(this Type type, bool throwForNonPublicValidators)
    {
        var isValidator = typeof(IValidator).IsAssignableFrom(type);
        if (isValidator)
        {
            if (throwForNonPublicValidators && !type.IsPublic)
            {
                throw new($"Found a non-public IMessage Validator - {type}");
            }
        }

        return isValidator;
    }

    static bool IsMessage(this Type type) =>
        typeof(IMessage).IsAssignableFrom(type) ||
        typeof(ICommand).IsAssignableFrom(type) ||
        typeof(IEvent).IsAssignableFrom(type);
}