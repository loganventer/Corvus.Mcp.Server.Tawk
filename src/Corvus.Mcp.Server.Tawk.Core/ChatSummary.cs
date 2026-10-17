namespace Corvus.Mcp.Server.Tawk.Core;

public sealed record ChatSummary(
    string Jid,
    string Name,
    bool IsGroup,
    int Unread,
    bool UnreadMention,
    bool Muted,
    bool Pinned,
    bool Archived,
    long LastTs,
    string? Preview);
