namespace Corvus.Mcp.Server.Tawk.Core.Sync;

public enum SyncOutcome
{
    Disabled,
    Busy,
    UpToDate,
    Pulled,
    Pushed,
    PulledAndPushed,
    Failed,
}
