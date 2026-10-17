using System.Text.Json.Nodes;
using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess;

/// <summary>
/// Runs a write. When tawk answers that the operation needs confirmation, asks the user through
/// <see cref="IUserConfirmation"/> and confirms or cancels it. The token never leaves the gate.
/// </summary>
public interface IConfirmationGate
{
    Task<ConfirmationOutcome> ExecuteAsync(
        string op,
        JsonObject? args,
        IUserConfirmation? confirmation,
        Action? onApprovalWaiting,
        CancellationToken cancellationToken);
}
