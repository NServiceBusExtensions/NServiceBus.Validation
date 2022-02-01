# <img src="/src/icon.png" height="25px"> NServiceBus Validation

[![Build status](https://ci.appveyor.com/api/projects/status/v9gfk5x5xo6kf46a/branch/main?svg=true)](https://ci.appveyor.com/project/SimonCropp/nservicebus-validation)
[![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.DataAnnotations.svg?label=NServiceBus.DataAnnotations)](https://www.nuget.org/packages/NServiceBus.DataAnnotations/)
[![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.FluentValidation.svg?label=NServiceBus.FluentValidation)](https://www.nuget.org/packages/NServiceBus.FluentValidation/)

Message validation using [DataAnnotations](https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.aspx) and [FluentValidation](https://github.com/JeremySkinner/FluentValidation).

<!--- StartOpenCollectiveBackers -->

[Already a Patron? skip past this section](#endofbacking)


## Community backed

**It is expected that all developers either [become a Patron](https://opencollective.com/nservicebusextensions/contribute/patron-6976) to use NServiceBusExtensions. [Go to licensing FAQ](https://github.com/NServiceBusExtensions/Home/#licensingpatron-faq)**


### Sponsors

Support this project by [becoming a Sponsor](https://opencollective.com/nservicebusextensions/contribute/sponsor-6972). The company avatar will show up here with a website link. The avatar will also be added to all GitHub repositories under the [NServiceBusExtensions organization](https://github.com/NServiceBusExtensions).


### Patrons

Thanks to all the backing developers. Support this project by [becoming a patron](https://opencollective.com/nservicebusextensions/contribute/patron-6976).

<img src="https://opencollective.com/nservicebusextensions/tiers/patron.svg?width=890&avatarHeight=60&button=false">

<a href="#" id="endofbacking"></a>

<!--- EndOpenCollectiveBackers -->


## NServiceBus.FluentValidation

Uses [FluentValidation](https://github.com/JeremySkinner/FluentValidation) to validate incoming and outgoing messages.


### NuGet package

https://www.nuget.org/packages/NServiceBus.FluentValidation/


### Usage

FluentValidation message validation can be enabled using the following:

<!-- snippet: FluentValidation -->
<a id='snippet-fluentvalidation'></a>
```cs
var validationConfig = endpointConfiguration.UseFluentValidation();
validationConfig.AddValidatorsFromAssemblyContaining<TheMessage>();
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L8-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-fluentvalidation' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

This will result in, when an invalid message being detected, a validation exception being thrown and that message being handled by [Recoverability](/nservicebus/recoverability/). The validation exception will also be added to [Unrecoverable exceptions](/nservicebus/recoverability/#unrecoverable-exceptions) to avoid unnecessary retries. <!-- singleLineInclude: validationexception. path: /src/validationexception.include.md -->

By default, incoming and outgoing messages are validated.

To disable for incoming messages use the following:

<!-- snippet: FluentValidation_disableincoming -->
<a id='snippet-fluentvalidation_disableincoming'></a>
```cs
endpointConfiguration.UseFluentValidation(
    incoming: false);
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L15-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-fluentvalidation_disableincoming' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

To disable for outgoing messages use the following:

<!-- snippet: FluentValidation_disableoutgoing -->
<a id='snippet-fluentvalidation_disableoutgoing'></a>
```cs
endpointConfiguration.UseFluentValidation(
    outgoing: false);
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L22-L27' title='Snippet source file'>snippet source</a> | <a href='#snippet-fluentvalidation_disableoutgoing' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Enabling validation on outgoing message will result in the validation exception be thrown in the context of the sender, instead of during message processing on the receiving endpoint. This can be particularly helpful in development and/or debugging scenarios since the stack trace and debugger will more accurately reflect the cause of the invalid message. <!-- singleLineInclude: validationoutgoing. path: /src/validationoutgoing.include.md -->

Messages can then have an associated [validator](https://github.com/JeremySkinner/FluentValidation/wiki/b.-Creating-a-Validator):

<!-- snippet: FluentValidation_message -->
<a id='snippet-fluentvalidation_message'></a>
```cs
public class TheMessage :
    IMessage
{
    public string Content { get; set; } = null!;
}

public class MyMessageValidator :
    AbstractValidator<TheMessage>
{
    public MyMessageValidator()
    {
        RuleFor(_ => _.Content)
            .NotEmpty();
    }
}
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/TheMessage.cs#L4-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-fluentvalidation_message' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Accessing the current pipeline context

In some cases a validator may need to use data from the current message context.

The current message context can be accessed via two extension methods:

 * The current [message headers](https://docs.particular.net/nservicebus/messaging/headers) can be accessed via `FluentValidationExtensions.Headers(this CustomContext customContext)`
 * The current `ContextBag` can be accessed via `FluentValidationExtensions.ContextBag(this CustomContext customContext)`.

<!-- snippet: FluentValidation_ContextValidator -->
<a id='snippet-fluentvalidation_contextvalidator'></a>
```cs
public class ContextValidator :
    AbstractValidator<TheMessage>
{
    public ContextValidator()
    {
        RuleFor(_ => _.Content)
            .Custom((propertyValue, validationContext) =>
            {
                var messageHeaders = validationContext.Headers();
                var bag = validationContext.ContextBag();
                if (propertyValue != "User" ||
                    messageHeaders.ContainsKey("Auth"))
                {
                    return;
                }
                validationContext.AddFailure("Expected Auth header to exist");
            });
    }
}
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/ContextValidator.cs#L5-L25' title='Snippet source file'>snippet source</a> | <a href='#snippet-fluentvalidation_contextvalidator' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Validator scanning

Validators are registered and resolved using [dependency injection](https://docs.particular.net/nservicebus/dependency-injection/). Assemblies can be added for validator scanning using either a generic Type, a Type instance, or an assembly instance.

<!-- snippet: FluentValidation_AddValidators -->
<a id='snippet-fluentvalidation_addvalidators'></a>
```cs
var validationConfig = endpointConfiguration.UseFluentValidation();
validationConfig.AddValidatorsFromAssemblyContaining<MyMessage>();
validationConfig.AddValidatorsFromAssemblyContaining(typeof(SomeOtherMessage));
validationConfig.AddValidatorsFromAssembly(assembly);
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L46-L53' title='Snippet source file'>snippet source</a> | <a href='#snippet-fluentvalidation_addvalidators' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Validator lifecycle can either be per endpoint ([Single instance](https://docs.particular.net/nservicebus/dependency-injection/)):

<!-- snippet: FluentValidation_EndpointLifecycle -->
<a id='snippet-fluentvalidation_endpointlifecycle'></a>
```cs
endpointConfiguration.UseFluentValidation(ValidatorLifecycle.Endpoint);
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L31-L35' title='Snippet source file'>snippet source</a> | <a href='#snippet-fluentvalidation_endpointlifecycle' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Or [instance per unit of work](https://docs.particular.net/nservicebus/dependency-injection/):

<!-- snippet: FluentValidation_UnitOfWorkLifecycle -->
<a id='snippet-fluentvalidation_unitofworklifecycle'></a>
```cs
endpointConfiguration.UseFluentValidation(ValidatorLifecycle.UnitOfWork);
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L37-L41' title='Snippet source file'>snippet source</a> | <a href='#snippet-fluentvalidation_unitofworklifecycle' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The default lifecycle is per endpoint.

By default, there are two exception scenarios when adding validators. An exception will be thrown if:

 * No validators are found in an assembly that is scanned.
 * Any non-public validators are found in an assembly that is scanned.

These exception scenarios can be excluded using the following:

<!-- snippet: FluentValidation_IgnoreValidatorConventions -->
<a id='snippet-fluentvalidation_ignorevalidatorconventions'></a>
```cs
var validationConfig = endpointConfiguration.UseFluentValidation();
validationConfig.AddValidatorsFromAssembly(assembly,
    throwForNonPublicValidators: false,
    throwForNoValidatorsFound: false);
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L58-L65' title='Snippet source file'>snippet source</a> | <a href='#snippet-fluentvalidation_ignorevalidatorconventions' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## NServiceBus.DataAnnotations

Uses [System.ComponentModel.DataAnnotations](https://msdn.microsoft.com/en-us/library/cc490428) to validate incoming and outgoing messages.


### NuGet package

https://www.nuget.org/packages/NServiceBus.DataAnnotations/


### Usage


DataAnnotations message validation can be enabled using the following:

<!-- snippet: DataAnnotations -->
<a id='snippet-dataannotations'></a>
```cs
configuration.UseDataAnnotationsValidation();
```
<sup><a href='/src/NServiceBus.DataAnnotations.Tests/Snippets/Usage.cs#L7-L11' title='Snippet source file'>snippet source</a> | <a href='#snippet-dataannotations' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

This will result in, when an invalid message being detected, a validation exception being thrown and that message being handled by [Recoverability](/nservicebus/recoverability/). The validation exception will also be added to [Unrecoverable exceptions](/nservicebus/recoverability/#unrecoverable-exceptions) to avoid unnecessary retries. <!-- singleLineInclude: validationexception. path: /src/validationexception.include.md -->

By default, incoming and outgoing messages are validated.

To disable for incoming messages use the following:

<!-- snippet: DataAnnotations_disableincoming -->
<a id='snippet-dataannotations_disableincoming'></a>
```cs
configuration.UseDataAnnotationsValidation(incoming: false);
```
<sup><a href='/src/NServiceBus.DataAnnotations.Tests/Snippets/Usage.cs#L13-L17' title='Snippet source file'>snippet source</a> | <a href='#snippet-dataannotations_disableincoming' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

To disable for outgoing messages use the following:

<!-- snippet: DataAnnotations_disableoutgoing -->
<a id='snippet-dataannotations_disableoutgoing'></a>
```cs
configuration.UseDataAnnotationsValidation(outgoing: false);
```
<sup><a href='/src/NServiceBus.DataAnnotations.Tests/Snippets/Usage.cs#L19-L23' title='Snippet source file'>snippet source</a> | <a href='#snippet-dataannotations_disableoutgoing' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Enabling validation on outgoing message will result in the validation exception be thrown in the context of the sender, instead of during message processing on the receiving endpoint. This can be particularly helpful in development and/or debugging scenarios since the stack trace and debugger will more accurately reflect the cause of the invalid message. <!-- singleLineInclude: validationoutgoing. path: /src/validationoutgoing.include.md -->

Messages can then be decorated with DataAnnotations attributes. For example, to make a property required use the [RequiredAttribute](https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.requiredattribute.aspx):

<!-- snippet: DataAnnotations_message -->
<a id='snippet-dataannotations_message'></a>
```cs
public class TheMessage :
    IMessage
{
    [Required]
    public string Content { get; set; } = null!;
}
```
<sup><a href='/src/NServiceBus.DataAnnotations.Tests/Snippets/TheMessage.cs#L4-L11' title='Snippet source file'>snippet source</a> | <a href='#snippet-dataannotations_message' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Icon

[Validation](https://thenounproject.com/term/validation/1680887/) designed by [Becris](https://thenounproject.com/Becris/) from [The Noun Project](https://thenounproject.com/).
