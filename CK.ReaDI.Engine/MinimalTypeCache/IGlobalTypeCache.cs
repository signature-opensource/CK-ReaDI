using System;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Xml.Linq;
using CK.Core;

namespace CK.ReaDI.Engine;

/// <summary>
/// Minimal contract of a global type cache dedicated to <c>[ReaDI]</c> types and optionally to other types
/// to manage type names used for logs and reporting.
/// <para>
/// This interface can be implemented by a much more complex cache than the <see cref="ReaDICache"/>.
/// </para>
/// </summary>
public interface IGlobalTypeCache
{
    /// <summary>
    /// Gets the cache of a <c>[ReaDI]</c> type.
    /// This must never return <c>null</c> on a class with at least one <see cref="ReaDIAttribute"/> in its attributes.
    /// </summary>
    /// <param name="type">A potential <c>[ReaDI]</c> type.</param>
    /// <returns>The cached type or null if the <paramref name="type"/> is not a <c>[ReaDI]</c> type.</returns>
    ICachedType? GetReaDICachedType( Type type );

    /// <summary>
    /// Gets the name to use for a type in logs and reporting.
    /// This can return null (the <see cref="TypeExtensions.ToCSharpName(Type?, bool, bool, bool)"/> will be used)
    /// but if the type is a <c>[ReaDI]</c> type it should return its <see cref="ICachedType.ReaDIName"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    string? GetReaDITypeName( Type type );
}
