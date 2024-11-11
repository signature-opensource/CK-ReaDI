using System.Collections.Immutable;
using System.Reflection;
using System.Text;

namespace CK.ReaDI.Engine;

/// <summary>
/// Member of a <see cref="ICachedType"/>.
/// The ReaDI engine ignores fields, this must include methods, properties and events.
/// <para>
/// This differs from the .Net model: a <see cref="ICachedType"/> is not a <see cref="ICachedMember"/>.
/// The <see cref="ICachedItem"/> is the generalization.
/// </para>
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

