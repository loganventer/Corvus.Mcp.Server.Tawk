using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.Managers;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

public sealed class FakeLiveUpdatesManager : ILiveUpdatesManager
{
    public TawkConnectionState ConnectionState { get; set; } = TawkConnectionState.Connected;

    public Task RunAsync(CancellationToken cancellationToken) => Task.Delay(Timeout.Infinite, cancellationToken);
}
