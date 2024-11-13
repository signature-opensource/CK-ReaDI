using CK.Core;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CK.ReaDI.Engine;

sealed class ReaDIMethodParameters
{
    internal readonly struct Parameter
    {
        readonly ParameterInfo _parameterInfo;
        readonly int _flags;

        public Parameter( ParameterInfo p, ParameterSourceBinding b, bool isOptional )
        {
            _parameterInfo = p;
            _flags = (int)b;
            if( isOptional ) _flags |= 64;
        }

        public ParameterInfo ParameterInfo => _parameterInfo;

        public ParameterSourceBinding SourceBinding => (ParameterSourceBinding)(_flags & 3);

        public bool IsOptional => (_flags & 64) != 0;
    }

    readonly ImmutableArray<Parameter> _parameters;
    readonly ImmutableArray<object?> _arguments;
    readonly int _idxReaDIAttribute;
    readonly int _idxCachedMember;
    readonly int _idxMemberHost;
    readonly int _idxCachedType;
    readonly int _idxTypeHost;
    readonly int _reaDIServiceCount;
    readonly ParameterSourceBinding _maxBinding;

    ReaDIMethodParameters( ImmutableArray<Parameter> parameters,
                           int idxReaDIAttribute,
                           int idxCachedMember,
                           int idxMemberHost,
                           int idxCachedType,
                           int idxTypeHost,
                           ParameterSourceBinding maxBinding,
                           int reaDIServiceCount,
                           ImmutableArray<object?> arguments )
    {
        _parameters = parameters;
        _idxReaDIAttribute = idxReaDIAttribute;
        _idxCachedMember = idxCachedMember;
        _idxMemberHost = idxMemberHost;
        _idxCachedType = idxCachedType;
        _idxTypeHost = idxTypeHost;
        _maxBinding = maxBinding;
        _reaDIServiceCount = reaDIServiceCount;
        _arguments = arguments;
    }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    public ImmutableArray<Parameter> Parameters => _parameters;

    /// <summary>
    /// Gets the number of ReaDI services. When 0, the method is an initial one, it can be called immediately.
    /// </summary>
    public int ReaDIServiceCount => _reaDIServiceCount;

    /// <summary>
    /// Gets the expected [ReaDI] attribute type.
    /// </summary>
    public Type? RequiredReaDIAttribute => _idxReaDIAttribute >= 0 ? _parameters[_idxReaDIAttribute].ParameterInfo.ParameterType : null;

    /// <summary>
    /// Gets whether the <see cref="ReaDITypeHost"/> or the <see cref="ReaDIMemberHost"/> must be provided.
    /// </summary>
    public bool RequiresTypeOrMemberHost => _maxBinding >= ParameterSourceBinding.TypeBoundService;

    /// <summary>
    /// Gets whether the <see cref="ReaDIMemberHost"/> must be provided.
    /// </summary>
    public bool RequiresMemberHost => _maxBinding >= ParameterSourceBinding.MemberBoundService;

    /// <summary>
    /// Creates an argument array if no <see cref="ReaDITypeHost"/> or the <see cref="ReaDIMemberHost"/> must be provided.
    /// </summary>
    /// <param name="attribute">The origin attribute. Must not be null if <see cref="RequiredReaDIAttribute"/> is not null.</param>
    /// <returns>The arguments with <see cref="ReaDIServiceCount"/> free slots.</returns>
    public object?[] CreateArguments( ReaDIAttribute? attribute )
    {
        Throw.DebugAssert( !RequiresTypeOrMemberHost && !RequiresMemberHost );
        return DoCreateArguments( attribute );
    }

    /// <summary>
    /// Creates an argument array if the <see cref="ReaDITypeHost"/> only must be provided.
    /// </summary>
    /// <param name="attribute">The origin attribute. Must not be null if <see cref="RequiredReaDIAttribute"/> is not null.</param>
    /// <returns>The arguments with <see cref="ReaDIServiceCount"/> free slots.</returns>
    public object?[] CreateArguments( ReaDIAttribute? attribute, ReaDITypeHost typeHost )
    {
        Throw.CheckState( RequiresTypeOrMemberHost && !RequiresMemberHost );
        var a = DoCreateArguments( attribute );
        if( _idxCachedType >= 0 )
        {
            a[_idxCachedType] = typeHost.Type;
        }
        if( _idxTypeHost >= 0 )
        {
            a[_idxTypeHost] = typeHost;
        }
        return a;
    }

    /// <summary>
    /// Creates an argument array if the <see cref="ReaDIMemberHost"/> must be provided.
    /// </summary>
    /// <param name="attribute">The origin attribute. Must not be null if <see cref="RequiredReaDIAttribute"/> is not null.</param>
    /// <returns>The arguments with <see cref="ReaDIServiceCount"/> free slots.</returns>
    public object?[] CreateArguments( ReaDIAttribute? attribute, ReaDIMemberHost memberHost )
    {
        Throw.CheckState( RequiresMemberHost );
        var a = DoCreateArguments( attribute );
        if( _idxCachedType >= 0 )
        {
            a[_idxCachedType] = memberHost.TypeHost.Type;
        }
        if( _idxTypeHost >= 0 )
        {
            a[_idxTypeHost] = memberHost.TypeHost;
        }
        if( _idxCachedMember >= 0 )
        {
            a[_idxCachedMember] = memberHost.Member;
        }
        if( _idxMemberHost >= 0 )
        {
            a[_idxMemberHost] = memberHost;
        }
        return a;
    }

    object?[] DoCreateArguments( ReaDIAttribute? attribute )
    {
        Throw.DebugAssert( RequiredReaDIAttribute == null || (attribute != null && RequiredReaDIAttribute.IsAssignableFrom( attribute.GetType() )) );
        var a = _arguments.ToArray();
        if( _idxReaDIAttribute >= 0 )
        {
            Throw.DebugAssert( attribute != null );
            a[_idxReaDIAttribute] = attribute;
        }
        return _arguments.ToArray();
    }

    internal static ReaDIMethodParameters? Create( IActivityMonitor monitor,
                                                   MethodBase info,
                                                   IServiceProvider services,
                                                   Func<string> errorName )
    {
        bool success = true;
        var infos = info.GetParameters();
        var parameters = new Parameter[infos.Length];
        var arguments = new object?[infos.Length];

        ParameterSourceBinding maxBinding = ParameterSourceBinding.External;
        int reaDIServiceCount = 0;
        int idxReaDIAttribute = -1;
        int idxCachedMember = -1;
        int idxMemberHost = -1;
        int idxCachedType = -1;
        int idxTypeHost = -1;
        for( int i = 0; i < infos.Length; i++ )
        {
            var pI = infos[i];
            var tI = pI.ParameterType;

            ParameterSourceBinding binding = ParameterSourceBinding.External;

            // We are on reference type: the fact that a default value exists, is how we consider the parameter to be optional.
            // Nullable Reference Type handling is not implemented: Thing? t = null is optional, but Thing? t is required.
            bool isOptional = pI.HasDefaultValue;

            // We reject: ValueType and object, IReaDIHost and ICachedItem (too general) and any MemberInfo.
            // Locally bound parameters can be ReaDITypeHost, ReaDIMemberHost, ICachedType or ICachedMember (or any of its specializations).
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
            else if( tI == typeof( IReaDIHost ) )
            {
                monitor.Error( $"""
                                {errorName()}: parameter 'object {pI.Name}' has type '{tI.Name}'.
                                [ReaDImpl] parameters must use ReaDITypeHost or ReaDIMemberHost.
                                """ );
                success = false;
            }
            else if( tI == typeof( ICachedItem ) || typeof( MemberInfo ).IsAssignableFrom( tI ) )
            {
                monitor.Error( $"""
                                {errorName()}: parameter 'object {pI.Name}' has type '{tI.Name}'.
                                [ReaDImpl] parameters must use ICachedType or ICachedMember (or one of its specializations).
                                """ );
                success = false;
            }
            else if( typeof( ICachedMember ).IsAssignableFrom( tI ) )
            {
                binding = ParameterSourceBinding.MemberHost;
                success &= CheckUniqueAndNonOptional( monitor, ref idxCachedMember, i, pI, tI, isOptional, parameters, errorName, "ICachedMember" );
            }
            else if( tI == typeof( ReaDIMemberHost ) )
            {
                binding = ParameterSourceBinding.MemberHost;
                success &= CheckUniqueAndNonOptional( monitor, ref idxMemberHost, i, pI, tI, isOptional, parameters, errorName, "ReaDIMemberHost" );
            }
            else if( tI == typeof( ICachedType ) )
            {
                binding = ParameterSourceBinding.TypeHost;
                success &= CheckUniqueAndNonOptional( monitor, ref idxCachedType, i, pI, tI, isOptional, parameters, errorName, "ICachedType" );
            }
            else if( tI == typeof( ReaDITypeHost ) )
            {
                binding = ParameterSourceBinding.TypeHost;
                success &= CheckUniqueAndNonOptional( monitor, ref idxTypeHost, i, pI, tI, isOptional, parameters, errorName, "ReaDITypeHost" );
            }
            else if( typeof( ReaDIAttribute ).IsAssignableFrom( tI ) )
            {
                success &= CheckUniqueAndNonOptional( monitor, ref idxReaDIAttribute, i, pI, tI, isOptional, parameters, errorName, "ReaDIAttribute" );
            }
            else if( typeof( IReaDIService ).IsAssignableFrom( tI ) )
            {
                ++reaDIServiceCount;
                binding = typeof( IReaDIMemberBoundService ).IsAssignableFrom( tI )
                            ? ParameterSourceBinding.MemberBoundService
                            : typeof( IReaDITypeBoundService ).IsAssignableFrom( tI )
                               ? ParameterSourceBinding.TypeBoundService
                               : typeof( ReaDIScope ).IsAssignableFrom( tI )
                                    ? ParameterSourceBinding.Scope
                                    : ParameterSourceBinding.Service;
            }
            else
            {
                var resolved = services.GetService( tI );
                if( resolved == null && !isOptional )
                {
                    monitor.Error( $"""
                                {errorName()}: parameter '{tI.Name} {pI.Name}' is an unresolved external service and is not optional.
                                Type '{tI:N}' cannot be resolved from the provided service provider.
                                """ );
                    success = false;

                }
                arguments[i] = resolved;
            }
            success &= CheckAmbiguity( monitor, errorName, infos, i, pI, tI );
            parameters[i] = new Parameter( pI, binding, isOptional );
            if( maxBinding < binding ) maxBinding = binding;
        }

        return success
                ? new ReaDIMethodParameters( ImmutableCollectionsMarshal.AsImmutableArray( parameters ),
                                             idxReaDIAttribute,
                                             idxCachedMember,
                                             idxMemberHost,
                                             idxCachedType,
                                             idxTypeHost,
                                             maxBinding,
                                             reaDIServiceCount,
                                             ImmutableCollectionsMarshal.AsImmutableArray( arguments ) )
                : null;

        static bool CheckUniqueAndNonOptional( IActivityMonitor monitor,
                                               ref int idxExists,
                                               int i,
                                               ParameterInfo pI,
                                               Type tI,
                                               bool isOptional,
                                               Parameter[] parameters,
                                               Func<string> errorName,
                                               string typeName )
        {
            bool success = true;
            if( idxExists >= 0 )
            {
                var exists = parameters[idxExists];
                monitor.Error( $"""
                                {errorName()}: duplicate {typeName} parameter '{tI.Name} {pI.Name}': '{exists.ParameterInfo.ParameterType.Name} {exists.ParameterInfo.Name}' already exists.
                                [ReaDImpl] parameters must have at most one {typeName} parameter.
                                """ );
                success = false;
            }
            else
            {
                idxExists = i;
            }
            if( isOptional )
            {
                monitor.Error( $"{errorName()}: parameter '{tI.Name} {pI.Name}' cannot be have a default value." );
                success = false;
            }
            return success;
        }

        static bool CheckAmbiguity( IActivityMonitor monitor,
                                    Func<string> errorName,
                                    ParameterInfo[] parameters,
                                    int i,
                                    ParameterInfo pI,
                                    Type tI )
        {
            bool success = true;
            for( int j = 0; j < i; j++ )
            {
                var tJ = parameters[j].ParameterType;
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


}
