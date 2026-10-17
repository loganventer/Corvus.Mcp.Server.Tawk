namespace Corvus.Mcp.Server.Tawk.Core;

public sealed record ChatUpdatedEvent(ChatSummary Chat) : TawkEvent;
