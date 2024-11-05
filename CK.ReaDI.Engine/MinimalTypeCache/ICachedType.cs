using System;
using System.Collections.Immutable;

namespace CK.ReaDI.Engine;

/// <summary>
/// Required cached type contract for the ReaDI engine.
/// <para>
/// Only reference types must be considered. Field or nested type members are ignored, only
/// methods, properties and events that appear in the <see cref="DeclaredMembers"/> are considered.
/// </para>
/// </summary>
public interface ICachedType : ICachedItem
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets the base type if any. <c>object</c> is irrelevant and should not appear.
    /// </summary>
    ICachedType? BaseType { get; }

    /// <summary>
    /// Gets the members declared by this type.
    /// This can contain other kind of members but only <c>[ReaDI]</c> decorated methods, properties or events are considered. 
    /// </summary>
    ImmutableArray<ICachedMember> DeclaredMembers { get; }
}
