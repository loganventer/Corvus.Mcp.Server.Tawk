using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Managers;

public interface ILiveUpdatesManager
{
    TawkConnectionState ConnectionState { get; }

    /// <summary>
    /// Reads events from tawk until cancelled, subscribes to every chat after each connect,
    /// and hands each update to every sink.
    /// </summary>
    Task RunAsync(CancellationToken cancellationToken);
}
