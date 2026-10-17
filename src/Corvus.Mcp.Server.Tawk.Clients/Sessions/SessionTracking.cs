using ModelContextProtocol.Server;

namespace Corvus.Mcp.Server.Tawk.Clients.Sessions;

/// <summary>An incoming message filter that remembers every session that talks to the server.</summary>
public static class SessionTracking
{
    public static McpMessageFilter Filter(IClientSessionRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);
        return next => async (context, cancellationToken) =>
        {
            registry.Add(new McpClientSession(context.Server));
            await next(context, cancellationToken).ConfigureAwait(false);
        };
    }
}
