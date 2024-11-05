using System;

namespace CK.Core;

/// <summary>
/// Declares an entry point for the ReaDI engine: the <see cref="ActualAttributeTypeAssemblyQualifiedName"/> is the
/// type name of an associated class that does the real job.
/// <para>
/// This enables maximal decoupling between runtime and engine assemblies:
/// the runtime assemblies can have absolutely no dependencies on any code generation or post production
/// infrastructure except this single type CK.ReaDI.Abstractions assembly.
/// </para>
/// <para>
/// This can be (and is often) used as a base class by specialized attributes.
/// </para>
/// <para>
/// This can only be applied to <see cref="ReaDITargets"/> (not to constructors, interfaces, delegates, fields, etc.).
/// </para>
/// </summary>
[AttributeUsage( ReaDITargets, AllowMultiple = true, Inherited = false )]
public class ReaDIAttribute : Attribute
{
    /// <summary>
    /// The DelegatedReaDIAttribute applicable targets.
    /// </summary>
    public const AttributeTargets ReaDITargets = AttributeTargets.Class
                                               | AttributeTargets.Enum
                                               | AttributeTargets.Method
                                               | AttributeTargets.Property
                                               | AttributeTargets.Event;

    /// <summary>
    /// Initializes a new <see cref="ReaDIAttribute"/> that delegates its behaviors to another object.
    /// </summary>
    /// <param name="actualAttributeTypeAssemblyQualifiedName">
    /// Assembly Qualified Name of the object that will be substitued to this attribute.
    /// Using the empty string here is allowed: this attribute defines a [ReaDI] type, just like the <see cref="ReaDITypeAttribute"/>.
    /// </param>
    public ReaDIAttribute( string actualAttributeTypeAssemblyQualifiedName )
    {
        ActualAttributeTypeAssemblyQualifiedName = actualAttributeTypeAssemblyQualifiedName;
    }

    /// <summary>
    /// Gets the Assembly Qualified Name of the public type that will be substitued to this attribute.
    /// <para>
    /// This must be a <c>[ReaDImpl]</c> class with an optional public <c>[ReaDImpl]</c> constructor (these are defined
    /// in CK.ReaDI.Engine).
    /// </para>
    /// </summary>
    public string ActualAttributeTypeAssemblyQualifiedName { get; private set; }
}

