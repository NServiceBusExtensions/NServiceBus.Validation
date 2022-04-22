using NServiceBus.ObjectBuilder;

class BuilderWrapper :
    IServiceProvider
{
    IBuilder builder;

    public BuilderWrapper(IBuilder builder) =>
        this.builder = builder;

    public object GetService(Type serviceType) =>
        builder.Build(serviceType);
}