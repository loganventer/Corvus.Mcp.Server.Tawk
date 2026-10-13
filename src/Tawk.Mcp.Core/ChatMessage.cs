namespace Tawk.Mcp.Core;

/// <summary>
/// One message. <see cref="PlayedAt"/> is when the recipient played a voice note or video the user sent, in Unix
/// seconds, and <see cref="DurationS"/> is the length of an audio or video message in seconds. Both are absent
/// until tawk reports them.
/// </summary>
public sealed record ChatMessage(
    string Id,
    string Chat,
    string? Sender,
    string? SenderName,
    bool FromMe,
    long Ts,
    string Type,
    string? Text,
    string? Status,
    bool Edited,
    bool Deleted,
    bool Forwarded,
    ReplyRef? ReplyTo,
    string? Reactions,
    LinkCard? Link,
    bool MentionsMe,
    long? PlayedAt = null,
    double? DurationS = null);
