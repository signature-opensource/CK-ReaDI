using CK.ReaDI.Engine;
using System.Collections.Immutable;
using System.Reflection;

namespace CK.Core;

public sealed class ReaDIMemberHost : IReaDIHost
{
    readonly ReaDITypeHost _typeHost;
    readonly ICachedMember _member;
    ImmutableArray<object> _attributes;
    readonly ReaDIHostKind _kind;

    public ReaDIMemberHost( ReaDITypeHost typeHost, ICachedMember member )
    {
        _typeHost = typeHost;
        _member = member;
    }

    internal void Finalize( ImmutableArray<object> attributes )
    {
        _attributes = attributes;
    }

    /// <summary>
    /// Gets the kind of this host.
    /// </summary>
    public ReaDIHostKind Kind => _kind;

    /// <summary>
    /// Gets the type host.
    /// </summary>
    public ReaDITypeHost TypeHost => _typeHost;

    ReaDIMemberHost? IReaDIHost.MemberHost => this;

    /// <summary>
    /// Gets the <see cref="ICachedMember"/> whose <see cref="ICachedMember.MemberInfo"/> can only be a <see cref="MethodInfo"/>,
    /// a <see cref="PropertyInfo"/> or a <see cref="EventInfo"/>.
    /// </summary>
    public ICachedMember Member => _member;

    /// <inheritdoc />
    public ImmutableArray<object> Attributes => _attributes;

    /// <summary>
    /// Overridden to return the <see cref="ICachedMember.ReaDIName"/>
    /// </summary>
    /// <returns>The member name.</returns>
    public override string ToString() => _member.ReaDIName;
}
