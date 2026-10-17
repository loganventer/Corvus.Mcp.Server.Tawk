namespace Corvus.Mcp.Server.Tawk.Core;

public sealed record ApprovalEvent(string RequestId, string State) : TawkEvent;
