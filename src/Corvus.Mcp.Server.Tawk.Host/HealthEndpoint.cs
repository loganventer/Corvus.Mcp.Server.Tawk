using Microsoft.AspNetCore.Http;
using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.Managers;

namespace Corvus.Mcp.Server.Tawk.Host;

public static class HealthEndpoint
{
    public const string Path = "/healthz";

    public static IResult Handle(ILiveUpdatesManager liveUpdates)
    {
        ArgumentNullException.ThrowIfNull(liveUpdates);
        return Results.Json(new { tawk = TawkConnectionStates.ToWire(liveUpdates.ConnectionState) });
    }
}
