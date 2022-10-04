﻿using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

var serviceCollection = new ServiceCollection();
var configuration = new EndpointConfiguration("DataAnnotationsValidationSample");
configuration.UsePersistence<LearningPersistence>();
configuration.UseTransport<LearningTransport>();
configuration.UseDataAnnotationsValidation(outgoing:false);

var endpointProvider = EndpointWithExternallyManagedServiceProvider
    .Create(configuration, serviceCollection);
await using var provider = serviceCollection.BuildServiceProvider();
var endpoint = await endpointProvider.Start(provider);

await endpoint.SendLocal(new MyMessage{Content = "sd"});
await endpoint.SendLocal(new MyMessage());

Console.WriteLine("Press any key to stop program");
Console.Read();
await endpoint.Stop();
