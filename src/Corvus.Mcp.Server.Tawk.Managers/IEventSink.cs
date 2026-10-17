using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Managers;

/// <summary>Somewhere live updates go: MCP resource notifications, a channel, an event stream.</summary>
public interface IEventSink
{
    Task OnUpdateAsync(LiveUpdate update, CancellationToken cancellationToken);
}
