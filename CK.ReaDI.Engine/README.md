# CK.ReaDI.Engine

This library implements the `[ReaDI]` attribute defined in `CK.ReaDI.Abstractions`.

This can be considered the engine of the CKomposable Engine: a CKomposable library only uses the 2 types
in `CK.ReaDI.Abstractions` and this one implements the real stuff.

This engine can be used directly but the "real" way to use it is from an independent Engine process.

## Method parameters restrictions: Reference Type only & disjoint types

> There must be no parameter in a `[ReaDImpl]` method whose type can be assignable from the type of another parameter.

This forbids duplicate types `M( T a, T b )`. Forbidden general case is `M( IAnimal a, IDog d )`
or `M( IAnimal a, Dog d )`. Moreover `object` parameter type is forbidden because
a `M( object o )` makes no sense.

Value types are also forbidden, only classes or interfaces are allowed.

