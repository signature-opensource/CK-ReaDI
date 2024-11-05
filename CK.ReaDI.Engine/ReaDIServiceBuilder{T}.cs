namespace CK.Core;

public abstract class ReaDIServiceBuilder<T> : ReaDI.Engine.ReaDIServiceBuilder where T : ReaDIService
{
    protected ReaDIServiceBuilder()
    {
    }

    private protected override sealed IReaDIResult BuildObject( IActivityMonitor monitor ) => Build( monitor );

    protected abstract ReaDIResult<T> Build( IActivityMonitor monitor );
}
