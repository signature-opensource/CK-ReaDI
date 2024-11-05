using CK.Core;
using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CK.ReaDI.Engine;

sealed class ReaDImplConstructor : ReaDImplMethodBase
{
    public ReaDImplConstructor( ConstructorInfo ctor,
                                ImmutableArray<ParameterInfo> parameterInfos,
                                ImmutableArray<Type> parameterTypes )
        : base( ctor, parameterInfos, parameterTypes ) 
    {
    }

    public ConstructorInfo ConstructorInfo => Unsafe.As<ConstructorInfo>( MethodBase );

    // A [ReaDI] constructor can have a:
    //  - a Type alone.
    //  - a MemberInfo alone.
    //  - a MethodInfo, PropertyInfo or EventInfo (if there's more than one of these, the resolution wil fail).
    //  - a Type and a MethodInfo, PropertyInfo or EventInfo.

    // From a [DelegatedReaDI] on a Type.
    // By submitting the Type we satisfy the Type alone and the MemberInfo. If the ReaDImplementation constructor expects a MethodInfo, PropertyInfo
    // or EventInfo, the resolution will never succeed.

    static bool IsTypeCompliant( ReaDImplType implType )
    {
        var ctorParams = implType.Contructor.ParameterTypes;
        return !ctorParams.Contains( typeof( MethodInfo ) )
               && !ctorParams.Contains( typeof( PropertyInfo ) )
               && !ctorParams.Contains( typeof( EventInfo ) );
    }

    internal static bool Create( IActivityMonitor monitor,
                                 ConstructorInfo info,
                                 ref ReaDImplConstructor? ctor )
    {
        bool success = true;
        if( ctor != null )
        {
            monitor.Error( $"More than one [ReaDI] constructor for '{info.DeclaringType:N}'. One and only one [ReaDI] constructor can be specified." );
            success = false;
        }

        string ErrorName() => $"[ReaDI] constructor of '{info.DeclaringType.ToCSharpName()}'";

        if( !info.IsPublic )
        {
            monitor.Error( $"{ErrorName()} must be public." );
            return false;
        }
        if( success && AnalyzeParameters( monitor, info, isConstructor: true, ErrorName, out ParameterInfo[] parameters, out Type[] arrayTypes ) )
        {
            ctor = new ReaDImplConstructor( info,
                                            ImmutableCollectionsMarshal.AsImmutableArray( parameters ),
                                            ImmutableCollectionsMarshal.AsImmutableArray( arrayTypes ) );
            return true;
        }
        return false;
    }

}
