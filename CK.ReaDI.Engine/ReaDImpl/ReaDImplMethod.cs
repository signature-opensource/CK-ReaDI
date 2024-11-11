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

    /// <summary>
    /// This applies only to non virtual methods that are masked by a method with the same name on a specialized type
    /// that must be decorated with a <see cref="OverrideReaDImplAttribute"/>.
    /// This substitution is possible regardless of the method protection: the parent method can be public, protected or private.
    /// </summary>
    ReaDImplMethod? MaskedBy { get; }

    internal static bool Create( IActivityMonitor monitor, MethodInfo info, [NotNullWhen( true )] out ReaDImplMethod? method )
    {
        string ErrorName() => $"Method [ReaDImpl] '{info.DeclaringType:N}.{info.Name}( ... )'";

        bool success = AnalyzeParameters( monitor, info, ErrorName, out ParameterInfo[] parameters, out Type[] arrayTypes );
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
            method = new ReaDImplMethod( info,
                                      ImmutableCollectionsMarshal.AsImmutableArray( parameters ),
                                      ImmutableCollectionsMarshal.AsImmutableArray( arrayTypes ) );
            return true;
        }
        method = null;
        return false;
    }
}
