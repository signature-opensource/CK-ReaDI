using CK.Core;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CK.ReaDI.Engine;

sealed class ReaDImplMethod : ReaDImplMethodBase 
{
    public ReaDImplMethod( MethodInfo method,
                           ImmutableArray<ParameterInfo> parameterInfos,
                           ImmutableArray<Type> parameterTypes )
        : base( method, parameterInfos, parameterTypes )
    {
    }

    public MethodInfo MethodInfo => Unsafe.As<MethodInfo>( MethodBase );

    public bool IsPublic => MethodBase.IsPublic;

    internal static ReaDImplMethod? Create( IActivityMonitor monitor, MethodInfo info, IServiceProvider services )
    {
        string ErrorName() => $"Method [ReaDImpl] '{info.DeclaringType:N}.{info.Name}( ... )'";

        var parameters = ReaDIMethodParameters.Create( monitor, info, services, ErrorName );
        bool success = parameters != null;
        var tR = info.ReturnType;
        if( !typeof( IReaDIResult ).IsAssignableFrom( tR ) )
        {
            monitor.Error( $"""
                            {ErrorName()} returns '{tR:C}'.
                            [ReaDImpl] method must return a ReaDIResult or a ReaDIResult<T>.
                            """ );
            success = false;
        }
        if( success ) 
        {
            Throw.DebugAssert( parameters != null );
            Type? tResult = null;
            var genArgs = tR.GetGenericArguments();
            if( genArgs.Length > 0 ) tResult = genArgs[0];

            method = new ReaDImplMethod( info,
                                      ImmutableCollectionsMarshal.AsImmutableArray( parameters ),
                                      ImmutableCollectionsMarshal.AsImmutableArray( arrayTypes ) );
            return true;
        }
        method = null;
        return false;
    }
}
