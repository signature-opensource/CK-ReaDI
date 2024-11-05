using CK.Core;

namespace CK.ReaDI.Engine;

interface IReaDIAction
{
    string Name { get; }

    ActionState State { get; }

    bool UseErrorTracker { get; }

    IReaDIResult Execute( IActivityMonitor monitor );
}
