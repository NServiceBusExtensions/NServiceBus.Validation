using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;

namespace NServiceBus.FluentValidation
{
    public static class TestHelper
    {
        public static IEnumerable<Type> FindMessagesWithoutValidator(params Assembly[] messageAssemblies)
        {
            Guard.AgainstNull(messageAssemblies, nameof(messageAssemblies));
            return messageAssemblies.SelectMany(FindMessagesWithoutValidator);
        }

        public static IEnumerable<Type> FindMessagesWithoutValidator(Assembly messageAssemblies)
        {
            Guard.AgainstNull(messageAssemblies, nameof(messageAssemblies));

            var messageTypes = messageAssemblies.GetTypes()
                .Where(p => p.IsMessage())
                .ToList();

            foreach (var validator in messageAssemblies.GetTypes().Where(p => p.IsValidator()))
            {
                // if a validator handles an IMessage remove that message type from the messageTypes list
                var interfaces = validator.GetInterfaces();
                var args = interfaces.Select(p => p.GenericTypeArguments)
                    .SelectMany(p => p)
                    .ToList();
                var messageType = args.FirstOrDefault(p => p.IsMessage());
                if (messageType != null)
                {
                    if (messageTypes.Contains(messageType))
                    {
                        messageTypes.Remove(messageType);
                    }
                }
            }

            return messageTypes;
        }

        public static IEnumerable<Type> FindHandledMessagesWithoutValidator(params Assembly[] handlerAssemblies)
        {
            Guard.AgainstNull(handlerAssemblies, nameof(handlerAssemblies));
            return handlerAssemblies.SelectMany(FindHandledMessagesWithoutValidator);
        }

        public static IEnumerable<Type> FindHandledMessagesWithoutValidator(Assembly handlerAssembly)
        {
            Guard.AgainstNull(handlerAssembly, nameof(handlerAssembly));
            List<(Type messageType, Type validatorOrHandler)> tracking = new();

            foreach (var t in handlerAssembly.GetClasses())
            {
                var interfaces = t.GetInterfaces();
                foreach (var argType in AllGenericArgs(interfaces))
                {
                    if (!argType.IsMessage())
                    {
                        continue;
                    }

                    if (t.IsValidator())
                    {
                        tracking.Add((messageType: argType, validatorOrHandler: typeof(IValidator)));
                    }

                    if (interfaces.Any(p => p.Name.Equals(typeof(IHandleMessages<>).Name)))
                    {
                        tracking.Add((messageType: argType, validatorOrHandler: typeof(IHandleMessages<>)));
                    }
                }
            }

            foreach (var messageGroup in tracking.GroupBy(p => p.messageType))
            {
                var handlerCount = messageGroup.Count(p => p.validatorOrHandler == typeof(IHandleMessages<>));
                var validatorCount = messageGroup.Count(p => p.validatorOrHandler == typeof(IValidator));

                if (handlerCount > 0 && validatorCount == 0)
                {
                    yield return messageGroup.Key;
                }
            }
        }

        static IEnumerable<Type> AllGenericArgs(Type[] interfaces)
        {
            return interfaces.Select(p => p.GenericTypeArguments).SelectMany(p => p);
        }

        static IEnumerable<Type> GetClasses(this Assembly handlerAssembly)
        {
            return handlerAssembly.GetTypes().Where(p => !p.IsInterface);
        }

        static bool IsValidator(this Type type)
        {
            var isValidator = typeof(IValidator).IsAssignableFrom(type);
            if (isValidator)
            {
                if (!type.IsPublic)
                {
                    throw new($"Found a non-public IMessage Validator - {type}");
                }
            }
            return isValidator;
        }

        static bool IsMessage(this Type type)
        {
            return typeof(IMessage).IsAssignableFrom(type) ||
                   typeof(ICommand).IsAssignableFrom(type) ||
                   typeof(IEvent).IsAssignableFrom(type) ;
        }
    }
}