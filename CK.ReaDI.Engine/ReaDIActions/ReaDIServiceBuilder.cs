using CK.Core;
using CK.ReaDI.Engine;

namespace CK.ReaDI.Engine;

public abstract class ReaDIServiceBuilder : ReaDIService, IReaDIAction
{
    ActionState _state;

    private protected ReaDIServiceBuilder()
    {
    }

    string IReaDIAction.Name
    {
        get
        {
            Throw.DebugAssert( _reaDIName != null );
            return _reaDIName;
        }
    }

    ActionState IReaDIAction.State => _state;

    bool IReaDIAction.UseErrorTracker => false;

    IReaDIResult IReaDIAction.Execute( IActivityMonitor monitor )
    {
        Throw.DebugAssert( _state is ActionState.Callable or ActionState.Done );
        if( _state == ActionState.Done ) return ReaDIResult.Success;
        _state = ActionState.Done;
        return BuildObject( monitor );
    }

    private protected abstract IReaDIResult BuildObject( IActivityMonitor monitor );

}
