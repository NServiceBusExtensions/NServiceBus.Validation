using NServiceBus.ObjectBuilder;

delegate CacheResult TryGetValidators(Type messageType, IBuilder builder);