using Corvus.Mcp.Library.Brokkr.Contracts.Managers;
using Corvus.Mcp.Server.Tawk.ResourceAccess.Memory;

namespace Corvus.Mcp.Server.Tawk.Managers.Sync;

/// <summary>Drops lapsed contact facts before each sync, so both machines compare the same rows and a lapsed fact never travels.</summary>
public sealed class PurgeExpiredFacts(IContactStore contacts) : IPreSyncHook
{
    public Task RunAsync(DateTimeOffset now, CancellationToken cancellationToken) => contacts.PurgeExpiredAsync(now, cancellationToken);
}
