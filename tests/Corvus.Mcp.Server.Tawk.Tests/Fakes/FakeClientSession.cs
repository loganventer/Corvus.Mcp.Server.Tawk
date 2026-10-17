using System.Text.Json.Nodes;
using Corvus.Mcp.Server.Tawk.Clients.Sessions;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

public sealed class FakeClientSession(string? clientName = "claude-code") : IClientSession
{
    public string? ClientName { get; } = clientName;

    public bool Broken { get; set; }

    public List<(string Method, JsonObject Parameters)> Sent { get; } = [];

    public Task SendNotificationAsync(string method, JsonObject parameters, CancellationToken cancellationToken)
    {
        if (Broken)
        {
            throw new IOException("session closed");
        }

        lock (Sent)
        {
            Sent.Add((method, parameters));
        }

        return Task.CompletedTask;
    }
}
