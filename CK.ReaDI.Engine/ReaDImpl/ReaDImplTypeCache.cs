using CK.Core;
using System;
using System.Collections.Generic;

namespace CK.ReaDI.Engine;

/// <summary>
/// ReaDImpl type cache.
/// </summary>
sealed class ReaDImplTypeCache
{
    readonly Dictionary<Type, ReaDImplType?> _implTypes;

    /// <summary>
    /// Initializes a new empty cache.
    /// </summary>
    public ReaDImplTypeCache()
    {
        _implTypes = new Dictionary<Type, ReaDImplType?>();
    }

    /// <summary>
    /// Tries to register a ReaDImplementation type: it must have a public [ReaDI] constructor and
    /// can have any number of public [ReaDI] methods. It can also have private or protected [ReaDI]
    /// methods that are continuations if and only if at least one public [ReaDI] method exists.
    /// <para>
    /// It is an error to try to register a type that has no public [ReaDI] constructor.
    /// </para>
    /// </summary>
    /// <param name="monitor">The monitor to use.</param>
    /// <param name="implType">The ReaDImpl type.</param>
    /// <returns>The ReaDImplType or null on error.</returns>
    public ReaDImplType? Register( IActivityMonitor monitor, Type implType )
    {
        Throw.DebugAssert( typeof( ReaDIService ).IsAssignableFrom( implType ) );
        if( !_implTypes.TryGetValue( implType, out var result ) )
        {
            result = ReaDImplType.Create( monitor, implType );
            _implTypes.Add( implType, result );
        }
        return result;
    }
}
