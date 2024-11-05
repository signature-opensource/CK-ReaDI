# CK.ReaDI.Abstractions

> **Important:** ReaDI is under developement. It is at a early R&D stage: currently the code may not even compile.
The final result may greatly differ than the initial vision but because we believe that this approach is really
interesting and will eventually be successful, we publish this ongoing work... early.


This library is absolutely ridiculous: it contains only the [`[ReaDI]`](./ReaDIAttribute.cs) and
[`[ReaDIType]`](./ReaDITypeAttribute.cs) attributes and has no dependency.
This is enough to support the real work that is done by the `CK.ReaDI.Engine` assembly.

The `[ReaDI]` attribute (most often a specialization) can decorate a Class, an Enum, a Method, a Property or an Event.
It targets an associated type that is its actual implementation thanks to its `ActualAttributeTypeAssemblyQualifiedName`
property.

It is concrete and can be used as-is but most often it is specialized to encapsulate the `ActualAttributeTypeAssemblyQualifiedName`
property in a nicer attribute that carries other properties (like a `[SqlProcedure( "CK.sActorCreate" )]`).

## The [ReaDI] types

Any public Enum or class that is decorated with a `[ReaDI]` are called **[ReaDI] types**.

They trigger actions implemented by their associated `[ReaDImpl]` type on the Engine side
during a CKomposition.

This enables maximal decoupling between runtime and engine assemblies: the runtime assemblies can
have absolutely no dependencies on any code generation or post production infrastructure except this minimal
`CK.ReaDI.Abstractions` assembly.

The [`[ReaDIType]`](./ReaDIType.cs) must be used when a type has no associated implementation but
one (or more) of its methods, properties or events are decorated with a `[ReaDI]` attribute and must
be handled by the engine.

This requirements has 2 goals: optimizing the discovery of ReaDI types (the type's `CustomAttributeData`
are enough to filter out non ReaDI types) and for maintenability: it is enough to look at the type to know
that it is a ReaDI type (no need to inspect all its methods, properties and events).

Note that any "empty" specialized attribute can be defined by using an empty string for the `ActualAttributeTypeAssemblyQualifiedName`.

## The ReaDI idea

On the Engine side, the associated types are `[ReaDImpl]` types. These classes
can have a public constructor also decorated with a `[ReaDImpl]` attribute and can have any number
of public or private methods also decorated with a `[ReaDImpl]` attribute.

`[ReaDImpl]` types (this attribute is defined in the [CK.ReaDI.Engine](../CK.ReaDI.Engine/CK.ReaDI.Engine.cs))
are the entry points of the whole CKomposition process. 

ReaDI is the underlying orchestrator of CKomposable engines.

The standard DI approach inverts the control of instance resolution and this is great because it solves both:
- The _strong type coupling_ of the `new` operator by externalizing the final type resolution.
  This allows type mappings and enables “Programming to Abstractions” way of thinking.
- The _hidden dependencies nightmare_ (the Service Locator anti-pattern) by exposing the
  dependencies (known as the “Hollywood Principle”).

But while this approach tackles how an instance is created and obtained, it doesn't address when it should or
can be created and/or activated.

Based on the “Package First” paradigm that states that an existing piece of code is necessarily used (the “No Dead Code” rule),
ReaDI takes a step further by supporting automatic activation of instances: in a sense this really is
the “Hollywood Principle” in action as it is the engine that is calling you, not a consequence of you resolving
a service from the DI container.

This applies to processes like compilers. Any process that must execute numerous steps based on one or more
inputs to produce one or more outputs must be correctly orchestrated. Centralization of the orchestration often
requires a plugin architecture to support extensions. A plugin architecture relies on one or more protocols
(often based on abstract contracts shared between the orchestrator – the Host – and extensions – the Plugins).
There’s nothing really complicated here, it is rather easy to setup... until multiple protocols are required
and plugins themselves require some kind of ordering and must be extended, introducing plugins of plugins
and subordinated orchestrators.

Our first implementations of the CKomposable framework (CKSetup, StObjEngine, etc.) was full of complex hooks
and orchestration started to be painful. ReadDI aims to offer the simplest possible pattern to reduce this
complexity to its bare minimum.

The idea is to consider the plugins as services (that have dependencies – via constructor injection) and
(some of) their methods as the steps (let's call them `ReaDIAction`) that must be executed.
`ReaDIAction` have their own additional dependencies that are simply the parameters of the method.

A service must be understood in a very generic manner (it can be any typed resource). There are 2 kind of services:
- The ones from a root DI container (a mere `IServiceProvider`). This container has no notion of scopes: the services
  it can resolve is a finite and immutable set of types that are unrelated to the ReaDI engine.
- The services that are `ReaDIService` can be created only by `ReaDIAction` and can be consumed by other
  `ReaDIAction`: they expose any kind of properties, helper methods (and even events).

Based on this, the core idea of ReaDI is that a `ReaDIAction` is automatically called when its dependencies are
available: the action is “readi” to be executed... There is no more orchestration code to write, only the
individual `ReaDIAction` methods and intermediate `ReaDIService` services.

Of course, there’s a little bit more to this simplified introduction:
- Dependencies may be optional.
- “Scopes” support “loops”, “foreach” on multiple inputs.
- “Continuations” enable a `ReaDIAction` to "call" other `ReaDIAction`. A complex process requiring multiple
  inputs (not necessarily available at the same time) can be split into smaller incremental pieces.

