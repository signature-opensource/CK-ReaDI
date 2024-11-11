using System.Collections.Immutable;
using System.Reflection;

namespace CK.ReaDI.Engine;

/// <summary>
/// Generalizes <see cref="ICachedType"/> and <see cref="ICachedMember"/>.
/// </summary>
public interface ICachedItem
{
    /// <summary>
    /// Gets the attribute data that decorate this type or member.
    /// </summary>
    ImmutableArray<CustomAttributeData> CustomAttributes { get; }

    /// <summary>
    /// Gets the attribute instances that decorate this type or member.
    /// </summary>
    ImmutableArray<object> Attributes { get; }

    /// <summary>
    /// Gets the a name that is enough for a user to recognize this member.
    /// <para>
    /// This is up to the cache implementation to decide how a member must be rendered.
    /// </para>
    /// </summary>
    string ReaDIName { get; }
}
