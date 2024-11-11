using System;
using System.Collections.Immutable;

namespace CK.Core;

public enum ReaDIServiceKind
{
    None,
    Service,
    ScopeService,
    BuilderService
}


public interface IReaDIEntryPoint
{
    Type Type { get; }

    ReaDIServiceKind ServiceKind { get; }
}

/// <summary>
/// Generalizes <see cref="ReaDITypeHost"/> and <see cref="ReaDIMemberHost"/>.
/// </summary>
public interface IReaDIHost
{
    /// <summary>
    /// Gets this kind of host.
    /// </summary>
    ReaDIHostKind Kind { get; }

    /// <summary>
    /// Gets the type host.
    /// </summary>
    ReaDITypeHost TypeHost { get; }

    /// <summary>
    /// Gets this member if this <see cref="Kind"/> is <see cref="ReaDIHostKind.Method"/>, <see cref="ReaDIHostKind.Property"/> or <see cref="ReaDIHostKind.Event"/>.
    /// </summary>
    ReaDIMemberHost? MemberHost { get; }

    /// <summary>
    /// Gets the "ReaDI aware" attributes that decorate this type or members.
    /// <para>
    /// This includes any regular custom attributes but not the <see cref="ReaDIAttribute"/> as
    /// they are replaced by their associated IReaDIHost instance.
    /// </para>
    /// </summary>
    ImmutableArray<object> Attributes { get; }

}
