using Corvus.Mcp.Library.Brokkr.Contracts.Managers;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

/// <summary>A sync manager that always reports the same thing, and counts how often it was asked.</summary>
public sealed class FixedSyncManager(SyncReport report) : ISyncManager
{
    public int Calls { get; private set; }

    public Task<SyncReport> SyncAsync(CancellationToken cancellationToken)
    {
        Calls++;
        return Task.FromResult(report);
    }
}
