using CK.Core;
using CommunityToolkit.HighPerformance;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace CK.ReaDI.Engine;

struct ReaDIInvokableImpl
{
    readonly ReaDImplMethodBase _method;
    readonly object?[] _args;
    int _missingCount;

    public ReaDIInvokableImpl( ReaDImplMethodBase method )
    {
        _method = method;
        _args = new object[_missingCount = method.ParameterTypes.Length];
    }

    public readonly int MissingCount => _missingCount;

    /// <summary>
    /// Simple relay to <see cref="MethodBase.Invoke(object?, object?[]?)"/>: current arguments remains encapsulated.
    /// </summary>
    /// <param name="instance">The instance. null when invoking a constructor.</param>
    /// <returns>The result (a <see cref="ReaDImplementation"/> for a constructor, a <see cref="IReaDIResult"/> for a method).</returns>
    public object? Invoke( object? instance ) => _method.MethodBase.Invoke( instance, BindingFlags.DoNotWrapExceptions, null, _args, null );

    /// <summary>
    /// Updates the argument at the given index. No type check.
    /// </summary>
    /// <param name="idxAttr">The argument position.</param>
    /// <param name="o">The argument.</param>
    internal void SetArgument( int idxAttr, object o )
    {
        Throw.DebugAssert( o != null );
        ref var instance = ref _args[idxAttr];
        if( instance == null ) --_missingCount;
        instance = o;
    }

    internal bool Initialize( IServiceProvider services )
    {
        var types = _method.ParameterTypes;
        for( int i = 0; i < types.Length; ++i )
        {
            ref var instance = ref _args[i];
            if( instance == null )
            {
                instance = services.GetService( types[i] );
                if( instance != null )
                {
                    --_missingCount;
                }
            }
        }
        return _missingCount == 0;
    }

    /// <summary>
    /// An object appeared. This returns true if a transition from <see cref="ActionState.NotReady"/> to
    /// <see cref="ActionState.Callable"/> must be done.
    /// </summary>
    /// <param name="o">The disappearing instance.</param>
    /// <returns>True if the arguments were not usable and are now fully specified.</returns>
    public bool OnAppear( Type t, object o )
    {
        Throw.DebugAssert( _missingCount > 0 );
        var types = _method.ParameterTypes;
        for( int i = 0; i < types.Length; ++i )
        {
            ref var instance = ref _args[i];
            if( instance == null )
            {
                var candidate = types[i];
                if( candidate.IsAssignableFrom( t ) )
                {
                    instance = o;
                    return --_missingCount == 0;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// An object disappeared. This returns true if a transition from <see cref="ActionState.Callable"/> to
    /// <see cref="ActionState.NotReady"/> must be done.
    /// </summary>
    /// <param name="o">The disappearing instance.</param>
    /// <returns>True if the arguments were fully specified (MissingCount = 0) and are now no more usable.</returns>
    public bool OnDisappear( object o )
    {
        Throw.DebugAssert( _missingCount > 0 );
        int idx = Array.IndexOf( _args, o );
        if( idx >= 0 )
        {
            _args[idx] = null;
            return _missingCount++ == 0;
        }
        return false;
    }

}
