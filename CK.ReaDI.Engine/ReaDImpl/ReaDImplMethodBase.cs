using CK.Core;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace CK.ReaDI.Engine;

sealed class Param
{
    Type ParameterType { get; }
    bool IsOptional { get; }
    ParameterInfo ParameterInfo { get; }
}

abstract class ReaDImplMethodBase
{
    readonly MethodBase _methodBase;
    readonly ImmutableArray<ParameterInfo> _parameterInfos;
    readonly ImmutableArray<Type> _parameterTypes;

    protected ReaDImplMethodBase( MethodBase method,
                                  ImmutableArray<ParameterInfo> parameterInfos,
                                  ImmutableArray<Type> parameterTypes )
    {
        _methodBase = method;
        _parameterInfos = parameterInfos;
        _parameterTypes = parameterTypes;
    }

    public MethodBase MethodBase => _methodBase;

    public ImmutableArray<ParameterInfo> ParameterInfos => _parameterInfos;

    public ImmutableArray<Type> ParameterTypes => _parameterTypes;

    private protected static bool AnalyzeParameters( IActivityMonitor monitor,
                                                     MethodBase info,
                                                     bool isConstructor,
                                                     Func<string> errorName,
                                                     out ParameterInfo[] parameters,
                                                     out Type[] arrayTypes )
    {
        bool success = true;
        parameters = info.GetParameters();
        arrayTypes = new Type[parameters.Length];
        if( parameters.Length == 0 )
        {
            monitor.Error( $"""
                            {errorName()} as no parameters.
                            [ReaDImpl] method or constructor must have parameters.
                            """ );
            success = false;
        }
        for( int i = 0; i < parameters.Length; i++ )
        {
            var pI = parameters[i];
            var tI = pI.ParameterType;
            if( tI.IsValueType )
            {
                monitor.Error( $"""
                                {errorName()}: parameter '{tI:C} {pI.Name}' is a value type.
                                [ReaDImpl] parameters must be reference type.
                                """ );
                success = false;
            }
            else if( tI == typeof( object ) )
            {
                monitor.Error( $"""
                                {errorName()}: parameter 'object {pI.Name}' has type 'object'.
                                [ReaDImpl] parameters cannot be object.
                                """ );
                success = false;
            }
            if( pI.HasDefaultValue )
            {
                monitor.Error( $"""
                                {errorName()}: parameter '{tI.Name} {pI.Name}' has a default value.
                                [ReaDImpl] parameters cannot have default value.
                                """ );
                success = false;
            }
            // Wrong!
            // A constructor can be deferred otherwise we won't be able to easyly define scoped context.
            // We should consider a [ReaDImpl] constructor as a ReaDIAction with a ReaDIResult<T> where T is
            // the declaring type.
            if( isConstructor )
            {
                if( tI == typeof( IReaDIHost ) || tI == typeof( ReaDITypeHost ) || tI == typeof( ReaDIMemberHost ) )
                {
                    monitor.Error( $"""
                                    {errorName()}: invalid parameter '{tI.Name} {pI.Name}'.
                                    [ReaDImpl] constructor parameters cannot be IReaDIHost, ReaDITypeHost or ReaDIMemberHost.
                                    Instead they can be ICachedType or ICachedMember.
                                    """ );
                    success = false;
                }
            }
            else
            {
                if( typeof( MemberInfo ).IsAssignableFrom( tI ) )
                {
                    monitor.Error( $"""
                                    {errorName()}: invalid parameter '{tI.Name} {pI.Name}'.
                                    [ReaDI] method parameters must use:
                                    - ReaDITypeHost instead of Type or ICachedType.
                                    - ReaDIMemberHost instead of MethodInfo, PropertyInfo, EventInfo or ICachedMember.
                                    - IReaDIHost instead of MemberInfo.
                                    """ );
                    success = false;
                }
            }
            success &= CheckAmbiguity( monitor, errorName, parameters, arrayTypes, i, pI, tI );
            arrayTypes[i] = tI;
        }
        return success;
    }

    static bool CheckAmbiguity( IActivityMonitor monitor,
                                Func<string> errorName,
                                ParameterInfo[] parameters,
                                Type[] arrayTypes,
                                int i,
                                ParameterInfo pI,
                                Type tI )
    {
        bool success = true;
        for( int j = 0; j < i; j++ )
        {
            var tJ = arrayTypes[j];
            bool jA = tJ.IsAssignableFrom( tI );
            bool iA = !jA && tI.IsAssignableFrom( tJ );
            if( iA || jA )
            {
                var pJ = parameters[j];
                if( tI == tJ )
                {
                    monitor.Error( $"""
                                {errorName()}: parameter '{pI.Name}' and '{pJ.Name}' have the same type.
                                [ReaDI] method parameters type must not be the same and be not assignable from each other.
                                """ );
                }
                else
                {
                    if( jA )
                    {
                        (tI, tJ) = (tJ, tI);
                        (pI, pJ) = (pJ, pI);
                    }
                    monitor.Error( $"""
                                {errorName()}: parameter '{tI:C} {pI.Name}' type is compatible with '{tJ:C} {pJ.Name}' type.
                                [ReaDI] method parameters type must not be assignable from each other.
                                """ );
                }
                success = false;
            }
        }
        return success;
    }
}
