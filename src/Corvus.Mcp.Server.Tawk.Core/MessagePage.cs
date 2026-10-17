namespace Corvus.Mcp.Server.Tawk.Core;

public sealed record MessagePage(ChatSummary Chat, IReadOnlyList<ChatMessage> Messages, long NextBefore);
