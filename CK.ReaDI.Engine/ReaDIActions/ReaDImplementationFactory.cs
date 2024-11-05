using CK.Core;
using System;
using System.Reflection;

namespace CK.ReaDI.Engine;

sealed class ReaDImplementationFactory : IReaDIAction
{
    ReaDImplType _implType;
    ReaDIInvokableImpl _args;
    ActionState _state;

    ReaDImplementationFactory( ReaDImplType implType, ReaDIAttribute attr, Type reaDIType, MemberInfo member, int idxAttr, IServiceProvider services )
    {
        Throw.DebugAssert( IsTypeCompliant( implType ) );
        _implType = implType;
        _args = new ReaDIInvokableImpl( _implType.Contructor );
        if( idxAttr >= 0 ) _args.SetArgument( idxAttr, attr );
        _args.OnAppear( typeof( Type ), reaDIType );
        if( _args.Initialize( services ) ) _state = ActionState.Callable;
    }


    public string Name => $"new {_implType.Name}";

    public ActionState State => _state;

    public bool UseErrorTracker => true;

    sealed class CtorResult : IReaDIResult
    {
        public CtorResult( ReaDIService created ) => Created = created;
        
        public bool HasError => false;

        public bool IsRetry => false;

        public bool IsSuccess => true;

        public string? MethodName => null;

        public ReaDIService? Created { get; }
    }

    public IReaDIResult Execute( IActivityMonitor monitor )
    {
        object? created = _args.Invoke( null );
        Throw.DebugAssert( "A constructor never returns null and the type is a ReaDIService.", created is ReaDIService );
        _state = ActionState.Called;
        return new CtorResult( (ReaDIService)created );
    }

    public static ReaDImplementationFactory? Create( IActivityMonitor monitor,
                                                     ReaDImplType reaDImplType,
                                                     Type reaDIType,
                                                     MemberInfo? reaDIMember,
                                                     ReaDIAttribute attr,
                                                     IServiceProvider services )
    {
        var tAttr = attr.GetType();
        int idxAttr = reaDImplType.Contructor.ParameterTypes.IndexOf( tAttr );
        if( idxAttr == -1 && tAttr != typeof( ReaDIAttribute ) )
        {
            monitor.Error( $"[ReaDI] constructor of '{reaDImplType.Type:N}': missing parameter '{tAttr:C} attribute' for the [ReaDI] attribute of type." );
            return null;
        }
        return new ReaDImplementationFactory( reaDImplType, attr, reaDIType, reaDIMember, idxAttr, services );
    }

}
