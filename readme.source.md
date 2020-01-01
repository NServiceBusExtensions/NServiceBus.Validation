<img src="/src/icon.png" height="25px"> Message validation using [DataAnnotations](https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.aspx) and [FluentValidation](https://github.com/JeremySkinner/FluentValidation).

[![Build status](https://ci.appveyor.com/api/projects/status/v9gfk5x5xo6kf46a/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/nservicebus-validation)
[![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.DataAnnotations.svg?label=NServiceBus.DataAnnotations)](https://www.nuget.org/packages/NServiceBus.DataAnnotations/)
[![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.FluentValidation.svg?label=NServiceBus.FluentValidation)](https://www.nuget.org/packages/NServiceBus.FluentValidation/)


<!--- StartOpenCollectiveBackers -->

[Already a Patron? skip past this section](#endofbacking)


## Community backed

**It is expected that all developers [become a Patron](https://opencollective.com/nservicebusextensions/order/6976) to use any of these libraries. [Go to licensing FAQ](https://github.com/NServiceBusExtensions/Home/#licensingpatron-faq)**


### Sponsors

Support this project by [becoming a Sponsors](https://opencollective.com/nservicebusextensions/order/6972). The company avatar will show up here with a link to your website. The avatar will also be added to all GitHub repositories under this organization.


### Patrons

Thanks to all the backing developers! Support this project by [becoming a patron](https://opencollective.com/nservicebusextensions/order/6976).

<img src="https://opencollective.com/nservicebusextensions/tiers/patron.svg?width=890&avatarHeight=60&button=false">

<!--- EndOpenCollectiveBackers -->

<a href="#" id="endofbacking"></a>


## NServiceBus.FluentValidation

Uses [FluentValidation](https://github.com/JeremySkinner/FluentValidation) to validate incoming and outgoing messages.


### NuGet package

https://www.nuget.org/packages/NServiceBus.FluentValidation/


### Usage

FluentValidation message validation can be enabled using the following:

snippet: FluentValidation

include: validationexception

By default, incoming and outgoing messages are validated.

To disable for incoming messages use the following:

snippet: FluentValidation_disableincoming

To disable for outgoing messages use the following:

snippet: FluentValidation_disableoutgoing

include: validationoutgoing

Messages can then have an associated [validator](https://github.com/JeremySkinner/FluentValidation/wiki/b.-Creating-a-Validator):

snippet: FluentValidation_message


### Accessing the current pipeline context

In some cases a validator may need to use data from the current message context.

The current message context can be accessed via two extension methods:

 * The current [message headers](https://docs.particular.net/nservicebus/messaging/headers) can be accessed via `FluentValidationExtensions.Headers(this CustomContext customContext)`
 * The current `ContextBag` can be accessed via `FluentValidationExtensions.ContextBag(this CustomContext customContext)`.

snippet: FluentValidation_ContextValidator


### Validator scanning

Validators are registered and resolved using [dependency injection](https://docs.particular.net/nservicebus/dependency-injection/). Assemblies can be added for validator scanning using either a generic Type, a Type instance, or an assembly instance.

snippet: FluentValidation_AddValidators

Validator lifecycle can either be per endpoint ([Single instance](https://docs.particular.net/nservicebus/dependency-injection/)):

snippet: FluentValidation_EndpointLifecycle

Or [instance per unit of work](https://docs.particular.net/nservicebus/dependency-injection/):

snippet: FluentValidation_UnitOfWorkLifecycle

The default lifecycle is per endpoint.

By default, there are two exception scenarios when adding validators. An exception will be thrown if:

 * No validators are found in an assembly that is scanned.
 * Any non-public validators are found in an assembly that is scanned.

These exception scenarios can be excluded using the following:

snippet: FluentValidation_IgnoreValidatorConventions


## NServiceBus.DataAnnotations

Uses [System.ComponentModel.DataAnnotations](https://msdn.microsoft.com/en-us/library/cc490428) to validate incoming and outgoing messages.


### NuGet package

https://www.nuget.org/packages/NServiceBus.DataAnnotations/


### Usage


DataAnnotations message validation can be enabled using the following:

snippet: DataAnnotations

include: validationexception

By default, incoming and outgoing messages are validated.

To disable for incoming messages use the following:

snippet: DataAnnotations_disableincoming

To disable for outgoing messages use the following:

snippet: DataAnnotations_disableoutgoing

include: validationoutgoing

Messages can then be decorated with DataAnnotations attributes. For example, to make a property required use the [RequiredAttribute](https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.requiredattribute.aspx):

snippet: DataAnnotations_message


## Release Notes

See [closed milestones](../../milestones?state=closed)


## Icon

[Validation](https://thenounproject.com/term/validation/1680887/) designed by [Becris](https://thenounproject.com/Becris/) from [The Noun Project](https://thenounproject.com/).