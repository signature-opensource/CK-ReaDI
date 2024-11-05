using System;

namespace CK.Core;

/// <summary>
/// Helper that must be used when a type has no associated <c>ReaDImplementation</c> type but
/// one of its methods, properties or events are decorated with <see cref="ReaDIAttribute"/>
/// and must be handled by the engine.
/// <para>
/// This requirements has 2 goals: optimizing the discovery of ReaDI types (the type's <see cref="System.Reflection.CustomAttributeData"/>
/// are enough to filter out non ReaDI types) and for maintenability: it is enough to look at the type to know that it is a ReaDI type
/// (no need to inspect all its methods, properties and events).
/// </para>
/// <para>
/// Note that any "empty" specialized attribute can be defined by using an empty string for
/// the <see cref="ReaDIAttribute.ActualAttributeTypeAssemblyQualifiedName"/>.
/// </para>
/// </summary>
[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
public sealed class ReaDITypeAttribute : ReaDIAttribute
{
    public ReaDITypeAttribute()
        : base( "" )
    {
    }
}
