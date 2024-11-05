using CK.ReaDI.Engine;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace CK.Core;


public sealed class ReaDIEngineBuilder
{
    readonly ReaDImplTypeCache _implCache;
    readonly IServiceProvider _services;
    readonly IGlobalTypeCache _typeCache;

    public ReaDIEngineBuilder( IServiceProvider services, IGlobalTypeCache typeCache )
    {
        _implCache = new ReaDImplTypeCache();
        _services = services;
        _typeCache = typeCache;
    }

    public bool Register( IActivityMonitor monitor, Type type )
    {
        bool success = true;
        var attributes = type.GetCustomAttributes( inherit: false );
        for( int i = 0; i < attributes.Length; i++ )
        {
            ref object a = ref attributes[i];
            if( a is ReaDIAttribute d )
            {
                var tImpl = SafeLoad( monitor, d.ActualAttributeTypeAssemblyQualifiedName );
                if( tImpl == null )
                {
                    success = false;
                }
                else
                {
                    if( !typeof(ReaDIService).IsAssignableFrom( tImpl ) )
                    {
                        monitor.Error( $"Invalid attribute [{d.GetType().Name}] on '{type:N}': the ActualAttributeTypeAssemblyQualifiedName '{d.ActualAttributeTypeAssemblyQualifiedName}' is not a " );
                    }
                    else
                    {
                        var reaDImplType = _implCache.Register( monitor, tImpl );
                        if( reaDImplType != null )
                        {
                            var action = ReaDImplementationFactory.Create( monitor, reaDImplType, type, null, d, _services );
                            if( action != null )
                            {
                                if( ac)
                            }
                        }
                    }
                }
            }
        }

        static Type? SafeLoad( IActivityMonitor monitor, string aqn )
        {
            try
            {
                return Type.GetType( aqn, throwOnError: true )!;
            }
            catch( Exception ex )
            {
                monitor.Error( ex );
                return null;
            }
        }

        if( !type.IsVisible )
        {
            monitor.Error( $"Type '{type:N}' is not visible outside of its assembly. It cannot be a [ReaDI] type." );
            return false;
        }
        if( !type.IsClass && !type.IsInterface && !type.IsEnum )
        {
            monitor.Error( $"Type '{type:N}' is not a class, an interface nor an enum. It cannot be a [ReaDI] type." );
            return false;
        }
        if( type.IsGenericTypeDefinition )
        {
            monitor.Error( $"Type '{type:N}' is a generic type definition. It cannot be a [ReaDI] type." );
            return false;
        }


    }


}
