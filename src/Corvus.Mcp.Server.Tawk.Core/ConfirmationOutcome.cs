using System.Text.Json;

namespace Corvus.Mcp.Server.Tawk.Core;

/// <summary>What happened to a write. Never carries a confirmation token.</summary>
public sealed record ConfirmationOutcome(ConfirmationStatus Status, JsonElement Result, string? Summary = null);
