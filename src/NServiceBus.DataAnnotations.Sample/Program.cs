using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

var serviceCollection = new ServiceCollection();
var configuration = new EndpointConfiguration("DataAnnotationsValidationSample");
configuration.UsePersistence<LearningPersistence>();
configuration.UseTransport<LearningTransport>();
configuration.UseDataAnnotationsValidation(outgoing:false);

var endpointWithExternallyManagedServiceProvider = EndpointWithExternallyManagedServiceProvider
    .Create(configuration, serviceCollection);
using var serviceProvider = serviceCollection.BuildServiceProvider();
var endpoint = await endpointWithExternallyManagedServiceProvider.Start(serviceProvider);

await endpoint.SendLocal(new MyMessage{Content = "sd"});
await endpoint.SendLocal(new MyMessage());

Console.WriteLine("Press any key to stop program");
Console.Read();
await endpoint.Stop();
