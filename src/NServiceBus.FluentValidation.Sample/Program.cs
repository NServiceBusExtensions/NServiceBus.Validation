using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

var services = new ServiceCollection();
var configuration = new EndpointConfiguration("FluentValidationSample");
configuration.UsePersistence<LearningPersistence>();
configuration.UseTransport<LearningTransport>();
configuration.UseFluentValidation(services, outgoing: false);
FluentValidationConfig.AddValidatorsFromAssemblyContaining<MyMessage>(services);

var endpointProvider = EndpointWithExternallyManagedServiceProvider
    .Create(configuration, services);
await using var provider = services.BuildServiceProvider();
var endpoint = await endpointProvider.Start(provider);

await endpoint.SendLocal(new MyMessage {Content = "sd"});
await endpoint.SendLocal(new MyMessage());

Console.WriteLine("Press any key to stop program");
Console.Read();
await endpoint.Stop();