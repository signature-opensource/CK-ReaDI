using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using CK.Core;

namespace CK.ReaDI.Engine;

/// <summary>
/// Simple implementation of the <see cref="IGlobalTypeCache"/>.
/// </summary>
public sealed partial class ReaDICache : IGlobalTypeCache
{
    readonly Dictionary<Type, ICachedType?> _cachedTypes;

    /// <summary>
    /// Initializes a new empty cache.
    /// </summary>
    public ReaDICache()
    {
        _cachedTypes = new Dictionary<Type, ICachedType?>();
    }

    /// <inheritdoc />
    public ICachedType? GetReaDICachedType( Type type )
    {
        if( !_cachedTypes.TryGetValue( type, out var result ) )
        {
            if( type.IsClass && !type.IsGenericTypeDefinition && !type.IsGenericParameter && !type.IsByRef )
            {
                var customAttributes = type.CustomAttributes.ToImmutableArray();
                if( HasReaDIAttribute( customAttributes ) )
                {
                    ICachedType? baseType = null;
                    var bT = type.BaseType;
                    if( bT != null && bT != typeof( object ) )
                    {
                        baseType = GetReaDICachedType( bT );
                    }
                    result = new CachedType( type, baseType, customAttributes );
                }
            }
            _cachedTypes.Add( type, result );
        }
        return result;
    }

    /// <inheritdoc />
    public string? GetReaDITypeName( Type type )
    {
        _cachedTypes.TryGetValue( type, out var t );
        return t?.ReaDIName ?? type.ToCSharpName();
    }

    /// <summary>
    /// Helper method that avoids attribute instanciation: this check if at least one <see cref="ReaDIAttribute"/>
    /// exists.
    /// </summary>
    /// <param name="attributes">Attributes data.</param>
    /// <returns>True if at least one <see cref="ReaDIAttribute"/> is found in the data attributes.</returns>
    public static bool HasReaDIAttribute( ImmutableArray<CustomAttributeData> attributes )
    {
        foreach( var data in attributes )
        {
            var t = data.AttributeType;
            do
            {
                if( t == typeof( ReaDIAttribute ) )
                {
                    return true;
                }
                Throw.DebugAssert( "We stop at 'object'.", t != null );
                t = t.BaseType;
            }
            while( t != typeof( object ) );
        }
        return false;
    }
}
