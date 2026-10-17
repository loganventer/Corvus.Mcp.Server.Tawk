namespace Corvus.Mcp.Server.Tawk.Core;

public sealed record UnreadSummary(int Total, int Mentions, IReadOnlyList<ChatSummary> Chats);
