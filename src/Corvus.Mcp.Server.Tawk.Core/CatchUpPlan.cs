namespace Corvus.Mcp.Server.Tawk.Core;

public sealed record CatchUpPlan(string Instructions, IReadOnlyList<ChatSummary> Chats);
