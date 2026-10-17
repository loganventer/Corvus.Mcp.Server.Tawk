using Corvus.Mcp.Server.Tawk.Core.Sync;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess.Sync;

/// <summary>Remembers, on this machine only, what was last seen in step with the remote.</summary>
public interface ISyncStateStore
{
    SyncState Load();

    void Save(SyncState state);
}
