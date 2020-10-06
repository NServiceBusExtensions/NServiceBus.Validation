# <img src="/src/icon.png" height="25px"> NServiceBus Validation

[![Build status](https://ci.appveyor.com/api/projects/status/v9gfk5x5xo6kf46a/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/nservicebus-validation)
[![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.DataAnnotations.svg?label=NServiceBus.DataAnnotations)](https://www.nuget.org/packages/NServiceBus.DataAnnotations/)
[![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.FluentValidation.svg?label=NServiceBus.FluentValidation)](https://www.nuget.org/packages/NServiceBus.FluentValidation/)

Message validation using [DataAnnotations](https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.aspx) and [FluentValidation](https://github.com/JeremySkinner/FluentValidation).


<!--- StartOpenCollectiveBackers -->

[Already a Patron? skip past this section](#endofbacking)


## Community backed

**It is expected that all developers either [become a Patron](https://opencollective.com/nservicebusextensions/contribute/patron-6976) or have a [Tidelift Subscription](#support-via-tidelift) to use NServiceBusExtensions. [Go to licensing FAQ](https://github.com/NServiceBusExtensions/Home/#licensingpatron-faq)**


### Sponsors

Support this project by [becoming a Sponsor](https://opencollective.com/nservicebusextensions/contribute/sponsor-6972). The company avatar will show up here with a website link. The avatar will also be added to all GitHub repositories under the [NServiceBusExtensions organization](https://github.com/NServiceBusExtensions).


### Patrons

Thanks to all the backing developers. Support this project by [becoming a patron](https://opencollective.com/nservicebusextensions/contribute/patron-6976).

<img src="https://opencollective.com/nservicebusextensions/tiers/patron.svg?width=890&avatarHeight=60&button=false">

<a href="#" id="endofbacking"></a>

<!--- EndOpenCollectiveBackers -->


## Support via TideLift

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-nservicebus.fluentvalidation?utm_source=nuget-nservicebus.fluentvalidation&utm_medium=referral&utm_campaign=enterprise).


<!-- toc -->
## Contents

  * [NServiceBus.FluentValidation](#nservicebusfluentvalidation)
    * [Usage](#usage)
    * [Accessing the current pipeline context](#accessing-the-current-pipeline-context)
    * [Validator scanning](#validator-scanning)
  * [NServiceBus.DataAnnotations](#nservicebusdataannotations)
    * [Usage](#usage-1)
  * [Security contact information](#security-contact-information)<!-- endToc -->


## NServiceBus.FluentValidation

Uses [FluentValidation](https://github.com/JeremySkinner/FluentValidation) to validate incoming and outgoing messages.


### NuGet package

https://www.nuget.org/packages/NServiceBus.FluentValidation/


### Usage

FluentValidation message validation can be enabled using the following:

<!-- snippet: FluentValidation -->
<a id='52d3d2ae'></a>
```cs
var validationConfig = endpointConfiguration.UseFluentValidation();
validationConfig.AddValidatorsFromAssemblyContaining<TheMessage>();
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L9-L14' title='Snippet source file'>snippet source</a> | <a href='#52d3d2ae' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

This will result in, when an invalid message being detected, a validation exception being thrown and that message being handled by [Recoverability](/nservicebus/recoverability/). The validation exception will also be added to [Unrecoverable exceptions](/nservicebus/recoverability/#unrecoverable-exceptions) to avoid unnecessary retries. <!-- singleLineInclude: validationexception. path: /src/validationexception.include.md -->

By default, incoming and outgoing messages are validated.

To disable for incoming messages use the following:

<!-- snippet: FluentValidation_disableincoming -->
<a id='131c3ef1'></a>
```cs
endpointConfiguration.UseFluentValidation(
    incoming: false);
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L16-L21' title='Snippet source file'>snippet source</a> | <a href='#131c3ef1' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

To disable for outgoing messages use the following:

<!-- snippet: FluentValidation_disableoutgoing -->
<a id='1a251dd8'></a>
```cs
endpointConfiguration.UseFluentValidation(
    outgoing: false);
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L23-L28' title='Snippet source file'>snippet source</a> | <a href='#1a251dd8' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Enabling validation on outgoing message will result in the validation exception be thrown in the context of the sender, instead of during message processing on the receiving endpoint. This can be particularly helpful in development and/or debugging scenarios since the stack trace and debugger will more accurately reflect the cause of the invalid message. <!-- singleLineInclude: validationoutgoing. path: /src/validationoutgoing.include.md -->

Messages can then have an associated [validator](https://github.com/JeremySkinner/FluentValidation/wiki/b.-Creating-a-Validator):

<!-- snippet: FluentValidation_message -->
<a id='d2bb81b0'></a>
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
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/TheMessage.cs#L4-L20' title='Snippet source file'>snippet source</a> | <a href='#d2bb81b0' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Accessing the current pipeline context

In some cases a validator may need to use data from the current message context.

The current message context can be accessed via two extension methods:

 * The current [message headers](https://docs.particular.net/nservicebus/messaging/headers) can be accessed via `FluentValidationExtensions.Headers(this CustomContext customContext)`
 * The current `ContextBag` can be accessed via `FluentValidationExtensions.ContextBag(this CustomContext customContext)`.

<!-- snippet: FluentValidation_ContextValidator -->
<a id='c7451940'></a>
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
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/ContextValidator.cs#L4-L24' title='Snippet source file'>snippet source</a> | <a href='#c7451940' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Validator scanning

Validators are registered and resolved using [dependency injection](https://docs.particular.net/nservicebus/dependency-injection/). Assemblies can be added for validator scanning using either a generic Type, a Type instance, or an assembly instance.

<!-- snippet: FluentValidation_AddValidators -->
<a id='f18e5509'></a>
```cs
var validationConfig = endpointConfiguration.UseFluentValidation();
validationConfig.AddValidatorsFromAssemblyContaining<MyMessage>();
validationConfig.AddValidatorsFromAssemblyContaining(typeof(SomeOtherMessage));
validationConfig.AddValidatorsFromAssembly(assembly);
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L45-L52' title='Snippet source file'>snippet source</a> | <a href='#f18e5509' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Validator lifecycle can either be per endpoint ([Single instance](https://docs.particular.net/nservicebus/dependency-injection/)):

<!-- snippet: FluentValidation_EndpointLifecycle -->
<a id='f6cf4e82'></a>
```cs
endpointConfiguration.UseFluentValidation(ValidatorLifecycle.Endpoint);
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L30-L34' title='Snippet source file'>snippet source</a> | <a href='#f6cf4e82' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Or [instance per unit of work](https://docs.particular.net/nservicebus/dependency-injection/):

<!-- snippet: FluentValidation_UnitOfWorkLifecycle -->
<a id='1872c4d9'></a>
```cs
endpointConfiguration.UseFluentValidation(ValidatorLifecycle.UnitOfWork);
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L36-L40' title='Snippet source file'>snippet source</a> | <a href='#1872c4d9' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The default lifecycle is per endpoint.

By default, there are two exception scenarios when adding validators. An exception will be thrown if:

 * No validators are found in an assembly that is scanned.
 * Any non-public validators are found in an assembly that is scanned.

These exception scenarios can be excluded using the following:

<!-- snippet: FluentValidation_IgnoreValidatorConventions -->
<a id='f087da29'></a>
```cs
var validationConfig = endpointConfiguration.UseFluentValidation();
validationConfig.AddValidatorsFromAssembly(assembly,
    throwForNonPublicValidators: false,
    throwForNoValidatorsFound: false);
```
<sup><a href='/src/NServiceBus.FluentValidation.Tests/Snippets/Usage.cs#L57-L64' title='Snippet source file'>snippet source</a> | <a href='#f087da29' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## NServiceBus.DataAnnotations

Uses [System.ComponentModel.DataAnnotations](https://msdn.microsoft.com/en-us/library/cc490428) to validate incoming and outgoing messages.


### NuGet package

https://www.nuget.org/packages/NServiceBus.DataAnnotations/


### Usage


DataAnnotations message validation can be enabled using the following:

<!-- snippet: DataAnnotations -->
<a id='e15ca415'></a>
```cs
configuration.UseDataAnnotationsValidation();
```
<sup><a href='/src/NServiceBus.DataAnnotations.Tests/Snippets/Usage.cs#L7-L11' title='Snippet source file'>snippet source</a> | <a href='#e15ca415' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

This will result in, when an invalid message being detected, a validation exception being thrown and that message being handled by [Recoverability](/nservicebus/recoverability/). The validation exception will also be added to [Unrecoverable exceptions](/nservicebus/recoverability/#unrecoverable-exceptions) to avoid unnecessary retries. <!-- singleLineInclude: validationexception. path: /src/validationexception.include.md -->

By default, incoming and outgoing messages are validated.

To disable for incoming messages use the following:

<!-- snippet: DataAnnotations_disableincoming -->
<a id='0fd4d1d6'></a>
```cs
configuration.UseDataAnnotationsValidation(incoming: false);
```
<sup><a href='/src/NServiceBus.DataAnnotations.Tests/Snippets/Usage.cs#L13-L17' title='Snippet source file'>snippet source</a> | <a href='#0fd4d1d6' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

To disable for outgoing messages use the following:

<!-- snippet: DataAnnotations_disableoutgoing -->
<a id='ba4872a0'></a>
```cs
configuration.UseDataAnnotationsValidation(outgoing: false);
```
<sup><a href='/src/NServiceBus.DataAnnotations.Tests/Snippets/Usage.cs#L19-L23' title='Snippet source file'>snippet source</a> | <a href='#ba4872a0' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Enabling validation on outgoing message will result in the validation exception be thrown in the context of the sender, instead of during message processing on the receiving endpoint. This can be particularly helpful in development and/or debugging scenarios since the stack trace and debugger will more accurately reflect the cause of the invalid message. <!-- singleLineInclude: validationoutgoing. path: /src/validationoutgoing.include.md -->

Messages can then be decorated with DataAnnotations attributes. For example, to make a property required use the [RequiredAttribute](https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.requiredattribute.aspx):

<!-- snippet: DataAnnotations_message -->
<a id='dbdcc0fc'></a>
```cs
public class TheMessage :
    IMessage
{
    [Required]
    public string Content { get; set; } = null!;
}
```
<sup><a href='/src/NServiceBus.DataAnnotations.Tests/Snippets/TheMessage.cs#L4-L11' title='Snippet source file'>snippet source</a> | <a href='#dbdcc0fc' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Validation](https://thenounproject.com/term/validation/1680887/) designed by [Becris](https://thenounproject.com/Becris/) from [The Noun Project](https://thenounproject.com/).
