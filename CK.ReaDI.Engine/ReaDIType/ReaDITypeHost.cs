using CK.ReaDI.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CK.Core;

/// <summary>
/// Associated instance to a <c>[ReaDI]</c> type: this exists only for type that are
/// decorated with at least one <c>[ReaDI]</c> attribute.
/// <para>
/// When the empty <c>[ReaDIType]</c> is used, at least one method, property or event
/// must be <c>[ReaDI]</c>.
/// </para>
/// </summary>
public sealed class ReaDITypeHost : IReaDIHost
{
    readonly ReaDITypeHost? _baseType;
    readonly ICachedType _type;
    ImmutableArray<object> _attributes;
    ImmutableArray<ReaDIMemberHost> _members;

    ReaDITypeHost( ReaDITypeHost? baseType, ICachedType type )
    {
        _baseType = baseType;
        _type = type;
    }

    internal void Finalize( ImmutableArray<ReaDIMemberHost> members, ImmutableArray<object> attributes )
    {
        _members = members;
        _attributes = attributes;
    }

    ReaDIHostKind IReaDIHost.Kind => ReaDIHostKind.Type;

    ReaDITypeHost IReaDIHost.TypeHost => this;

    ReaDIMemberHost? IReaDIHost.MemberHost => null;

    /// <summary>
    /// Gets the type.
    /// </summary>
    public ICachedType Type => _type;

    /// <inheritdoc />
    public ImmutableArray<object> Attributes => _attributes;

    /// <summary>
    /// Gets the "ReaDI aware" members.
    /// </summary>
    public ImmutableArray<ReaDIMemberHost> Members => _members;

    /// <summary>
    /// Gets all the <see cref="IReaDIHost"/> that this type "contains" in the following order:
    /// <list type="number">
    ///     <item>if <paramref name="includeBaseTypes"/> is true, the hosts from the base types, from the most general one up to the direct base type.</item>
    ///     <item>this <see cref="ReaDITypeHost"/>.</item>
    ///     <item>all this <see cref="Members"/>.</item>
    /// </list>
    /// </summary>
    /// <param name="includeBaseTypes">True to include the base types.</param>
    public IEnumerable<IReaDIHost> GetAllHosts( bool includeBaseTypes )
    {
        if( includeBaseTypes && _baseType != null )
        {
            foreach( var h in _baseType.GetAllHosts( includeBaseTypes ) )
            {
                yield return h;
            }
        }
        yield return this;
        foreach( var member in _members )
        {
            yield return member;
        }
    }

    /// <summary>
    /// Gets all the "ReaDI aware" attributes for all the hosts that this type "contains".
    /// See <see cref="GetAllHosts(bool)"/> and <see cref="IReaDIHost.Attributes"/>.
    /// </summary>
    public IEnumerable<object> GetAllAttributes( bool includeBaseTypes )
    {
        foreach( var host in GetAllHosts( includeBaseTypes ) )
        {
            foreach( var o in host.Attributes )
            {
                yield return o;
            }
        }
    }

    /// <summary>
    /// Overridden to return the <see cref="ICachedType.ReaDIName"/>
    /// </summary>
    /// <returns>The type name.</returns>
    public override string ToString() => _type.ReaDIName;
}
