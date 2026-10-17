using System.Text.Json;
using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.ResourceAccess;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

/// <summary>Waiting requests that are whatever a test says, and approvals that answer or refuse as told.</summary>
public sealed class FakeTawkApprovals : ITawkApprovals
{
    public List<WaitingRequest> Waiting { get; } = [];

    public List<(string Id, string Token)> Approved { get; } = [];

    public TawkControlException? Refusal { get; set; }

    public IReadOnlyList<WaitingRequest> TakeWaiting() => Waiting.ToList();

    public Task<JsonElement> ApproveAsync(string requestId, string adminToken, CancellationToken cancellationToken)
    {
        if (Refusal is not null)
        {
            throw Refusal;
        }

        Approved.Add((requestId, adminToken));
        return Task.FromResult(JsonDocument.Parse("""{"id":"3EB0D41C22"}""").RootElement.Clone());
    }
}
