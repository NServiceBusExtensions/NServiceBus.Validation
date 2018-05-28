using System;
using System.Collections.Concurrent;
using FluentValidation;

static class ValidatorTypeCache
{
    static ConcurrentDictionary<Type, Type> typeCache = new ConcurrentDictionary<Type, Type>();

    static Type validatorType = typeof(IValidator<>);

    public static Type Find(Type messageType)
    {
        return typeCache.GetOrAdd(messageType, type => validatorType.MakeGenericType(type));
    }
}