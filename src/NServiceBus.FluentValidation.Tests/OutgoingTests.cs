using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.FluentValidation;

[UsesVerify]
public class OutgoingTests
{
    [Fact]
    public Task With_no_validator()
    {
        var message = new MessageWithNoValidator();
        return Send(message);
    }

    [Fact]
    public Task With_no_validator_Fallback()
    {
        var message = new MessageWithNoValidator();
        return ThrowsTask(() => Send(message, fallback: _ => new FallbackValidator()));
    }

    class FallbackValidator : AbstractValidator<MessageWithNoValidator>
    {
        public FallbackValidator() =>
            RuleFor(_ => _.Content).NotEmpty();
    }

    [Fact]
    public Task With_validator_valid()
    {
        var message = new MessageWithValidator
        {
            Content = "content"
        };
        return Send(message);
    }

    [Fact]
    public Task With_uow_validator()
    {
        var message = new MessageWithValidator();
        return ThrowsTask(() => Send(message, ValidatorLifecycle.UnitOfWork));
    }

    [Fact]
    public Task With_validator_invalid()
    {
        var message = new MessageWithValidator();
        return ThrowsTask(() => Send(message));
    }

    [Fact]
    public Task With_async_validator_valid()
    {
        var message = new MessageWithAsyncValidator
        {
            Content = "content"
        };
        return Send(message);
    }

    [Fact]
    public Task With_async_validator_invalid()
    {
        var message = new MessageWithAsyncValidator();
        return Assert.ThrowsAsync<MessageValidationException>(() => Send(message));
    }

    static async Task Send(
        object message,
        ValidatorLifecycle lifecycle = ValidatorLifecycle.Endpoint,
        [CallerMemberName] string key = "",
        Func<Type, IValidator>? fallback = null)
    {
        var configuration = new EndpointConfiguration("FluentValidationOutgoing" + key);
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        configuration.DisableFeature<TimeoutManager>();
        configuration.DisableFeature<Sagas>();

        var validation = configuration.UseFluentValidation(lifecycle, incoming: false, fallback: fallback);
        validation.AddValidatorsFromAssemblyContaining<MessageWithNoValidator>(throwForNonPublicValidators: false);

        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(message);
    }
}