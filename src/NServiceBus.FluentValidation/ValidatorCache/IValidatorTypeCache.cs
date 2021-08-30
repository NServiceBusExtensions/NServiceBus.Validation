using System;
using System.Collections.Generic;
using FluentValidation;
using NServiceBus.ObjectBuilder;

delegate bool TryGetValidators(Type messageType, IBuilder builder, out IEnumerable<IValidator> validators);