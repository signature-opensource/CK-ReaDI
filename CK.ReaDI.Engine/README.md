# CK.ReaDI.Engine

This library implements the `[ReaDI]` attribute support defined in `CK.ReaDI.Abstractions`.

This can be considered the engine of the CKomposable Engine: a CKomposable library only uses the 2 types
in `CK.ReaDI.Abstractions` and this one implements the real stuff.

This engine can be used directly but the "real" way to use it is from an independent Engine process.

## [ReaDI] entry points.
Classes in the runtime space decorated with one or more `[ReaDI]` attributes (on the type, its methods, properties
and/or events) define the entry points for the engine. A [ReaDI] type defines one or more entry points, one for
each `[ReaDI]` attribute that decorate it. There are 2 kind of entry points. An entry point can be:

- A `[ReaDImpl]` decorated public or internal class. This is called a `[ReaDImpl]` type.
- A `ReaDIService` or `ReaDIScope` specialization. 

These 2 kind of entry points are quite different.
- A `[ReaDImpl]` type is not a `ReaDIService`, it is not injectable in other `ReaDIService` or `ReaDIAction` but
  captures its source that is the `[ReaDI]` attribute and the decorated member through its single `[ReaDImpl]`
  constructor. This constructor is immediately resolvable because its parameters cannot be `ReaDIService`.

  All such `[ReaDImpl]` instances are immediately resolved and are available in a `ReaDITypeHost` object that
  represents the source `[ReaDI]` type and hosts all the `[ReaDImpl]` instances 




## Method parameters restrictions: Reference Type only & disjoint types

> There must be no parameter in a `[ReaDImpl]` method whose type can be assignable from the type of another parameter.

This forbids duplicate types `M( T a, T b )`. Forbidden general case is `M( IAnimal a, IDog d )`
or `M( IAnimal a, Dog d )`. Moreover `object` parameter type is forbidden because
a `M( object o )` makes no sense.

Value types are also forbidden, only classes or interfaces are allowed.

