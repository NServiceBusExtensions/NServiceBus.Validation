using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;

namespace NServiceBus.FluentValidation
{
    public static class TestHelper
    {
        public static IEnumerable<Type> FindHandledMessagesWithoutValidator(params Assembly[] handlerAssemblies)
        {
            Guard.AgainstNull(handlerAssemblies, nameof(handlerAssemblies));
            return handlerAssemblies.SelectMany(FindHandledMessagesWithoutValidator);
        }

        public static IEnumerable<Type> FindHandledMessagesWithoutValidator(Assembly handlerAssembly)
        {
            Guard.AgainstNull(handlerAssembly, nameof(handlerAssembly));
            var tracking = new List<(Type messageType, Type validatorOrHandler)>();

            foreach (var t in handlerAssembly.GetTypes().Where(p => !p.IsInterface))
            {
                var interfaces = t.GetInterfaces();
                foreach (var argType in interfaces.Select(p => p.GenericTypeArguments).SelectMany(p => p))
                {
                    if (!typeof(IMessage).IsAssignableFrom(argType))
                    {
                        continue;
                    }

                    if (typeof(IValidator).IsAssignableFrom(t))
                    {
                        if (!t.IsPublic)
                        {
                            throw new Exception($"Found a non-public IMessage Validator - {t}");
                        }

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
    }
}