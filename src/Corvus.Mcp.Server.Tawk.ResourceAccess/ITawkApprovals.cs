using System.Text.Json;
using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess;

/// <summary>The writes of this instance that tawk queued for an answer, and the way to answer one as admin.</summary>
public interface ITawkApprovals
{
    /// <summary>Those still waiting, and once each those tawk answered in the meantime.</summary>
    IReadOnlyList<WaitingRequest> TakeWaiting();

    /// <summary>
    /// Answers the waiting request with tawk's admin token and returns what the request itself then answered.
    /// Throws <see cref="TawkControlException"/> when tawk refuses; the request then still waits for the user.
    /// </summary>
    Task<JsonElement> ApproveAsync(string requestId, string adminToken, CancellationToken cancellationToken);
}
