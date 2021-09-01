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
            return messageAssemblies.SelectMany(assembly => FindMessagesWithoutValidator(assembly));
        }

        public static IEnumerable<Type> FindMessagesWithoutValidator(Assembly messageAssemblies, bool throwForNonPublicValidators = true)
        {
            var messageTypes = messageAssemblies.GetTypes()
                .Where(p => p.IsMessage())
                .ToList();

            foreach (var validator in messageAssemblies.GetTypes().Where(p => p.IsValidator(throwForNonPublicValidators)))
            {
                // if a validator handles an IMessage remove that message type from the messageTypes list
                var interfaces = validator.GetInterfaces();
                var args = interfaces.Select(p => p.GenericTypeArguments)
                    .SelectMany(p => p)
                    .ToList();
                var messageType = args.FirstOrDefault(p => p.IsMessage());
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

        public static IEnumerable<Type> FindHandledMessagesWithoutValidator(params Assembly[] handlerAssemblies)
        {
            return handlerAssemblies.SelectMany(assembly => FindHandledMessagesWithoutValidator(assembly));
        }

        public static IEnumerable<Type> FindHandledMessagesWithoutValidator(Assembly handlerAssembly, bool throwForNonPublicValidators = true)
        {
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

                    if (t.IsValidator(throwForNonPublicValidators))
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

        static bool IsMessage(this Type type)
        {
            return typeof(IMessage).IsAssignableFrom(type) ||
                   typeof(ICommand).IsAssignableFrom(type) ||
                   typeof(IEvent).IsAssignableFrom(type) ;
        }
    }
}