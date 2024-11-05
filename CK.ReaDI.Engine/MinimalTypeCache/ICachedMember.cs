using System.Collections.Immutable;
using System.Reflection;

namespace CK.ReaDI.Engine;

/// <summary>
/// Member of a <see cref="ICachedType"/>.
/// The ReaDI engine ignores fields, this must include methods, properties and events.
/// </summary>
public interface ICachedMember : ICachedItem
{
    /// <summary>
    /// Gets the cached <see cref="System.Reflection.MemberInfo"/>.
    /// </summary>
    MemberInfo MemberInfo { get; }

    /// <summary>
    /// Gets the type that declares this member.
    /// </summary>
    ICachedType DeclaringType { get; }
}
