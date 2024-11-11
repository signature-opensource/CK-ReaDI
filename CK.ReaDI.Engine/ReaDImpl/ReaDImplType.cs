using CK.Core;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CK.ReaDI.Engine;

/// <summary>
/// A ReaDImplType is either a <see cref="ReaDIService"/> or a <c>[ReadImpl]</c> entry point implementation.
/// <para>
/// In both cases the type must be sealed.
/// </para>
/// <para>
/// When the type is a ReaDIService, it must not have a <c>[ReadImpl]</c> constructor (because it is
/// created by a <c>[ReadImpl]</c> method) and may have no <c>[ReadImpl]</c> method at all: The service offers
/// its functionalities to others but has no ReaDI actions of its own.
/// </para>
/// <para>
/// When the type is a <c>[ReadImpl]</c> entry point implementation, it can have at most one <c>[ReadImpl]</c> constructor.
/// If it has no constructor, it must have at least one <c>[ReadImpl]</c> method.
/// </para>
/// </summary>
sealed class ReaDImplType
{
    readonly Type _type;
    readonly ReaDImplConstructor? _contructor;
    readonly ImmutableArray<ReaDImplMethod> _methods;

    ReaDImplType( Type type, ReaDImplConstructor? contructor, ImmutableArray<ReaDImplMethod> methods )
    {
        _type = type;
        _contructor = contructor;
        _methods = methods;
    }

    public Type Type => _type;

    public ReaDImplConstructor? Contructor => _contructor;

    public ImmutableArray<ReaDImplMethod> Methods => _methods;

    public string Name => _type.Name;

    public static ReaDImplType? Create( IActivityMonitor monitor, Type t )
    {
        bool success = true;
        ReaDImplConstructor? constructor = null;
        ImmutableArray<ReaDImplMethod>.Builder? methods = null;
        bool hasPublicMethod = false;
        bool hasPrivateMethod = false;
        foreach( var member in t.GetMembers( BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic ) )
        {
            if( member is not MethodBase m || m.IsSpecialName ) continue;
            foreach( var a in m.CustomAttributes )
            {
                if( a.AttributeType == typeof( ReaDImplAttribute ) )
                {
                    if( m is ConstructorInfo cInfo )
                    {
                        success &= ReaDImplConstructor.Create( monitor, cInfo, ref constructor );
                    }
                    else
                    {
                        Throw.DebugAssert( member is MethodInfo );
                        success &= ReaDImplMethod.Create( monitor, Unsafe.As<MethodInfo>( member ), out var method );
                        if( success )
                        {
                            Throw.DebugAssert( method != null );
                            if( method.IsPublic ) hasPublicMethod = true;
                            else hasPrivateMethod = true;
                            methods ??= ImmutableArray.CreateBuilder<ReaDImplMethod>();
                            methods.Add( method );
                        }
                    }
                }
            }
        }
        if( success )
        {
            if( constructor == null )
            {
                monitor.Error( $"No public [ReaDImpl] constructor can be found for '{t:N}'." );
                success = false;
            }
            if( hasPrivateMethod && !hasPublicMethod )
            {
                Throw.DebugAssert( methods != null );
                monitor.Error( $"""
                                [ReaDImpl] type '{t:N}' has private [ReaDImpl] methods but no public [ReaDImpl] method.
                                Private method ('{methods.Select( m => m.MethodInfo.Name).Concatenate("', '")}') can only be continuations of at least one public [ReaDImpl] method.
                                """ );
                success = false;
            }
            if( success )
            {
                Throw.DebugAssert( constructor != null );
                return new ReaDImplType( t, constructor, methods != null ? methods.ToImmutableArray() : ImmutableArray<ReaDImplMethod>.Empty );
            }
        }
        return null;
    }
}
