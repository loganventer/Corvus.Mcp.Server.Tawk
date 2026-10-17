using Corvus.Mcp.Server.Tawk.Core.Sync;

namespace Corvus.Mcp.Server.Tawk.Managers.Sync;

/// <summary>Keeps this machine's memory in step with the remote copy.</summary>
public interface IMemorySyncManager
{
    /// <summary>One cycle: merge what the remote has, then push if this machine holds something the remote does not.</summary>
    Task<SyncReport> SyncAsync(CancellationToken cancellationToken);
}
