namespace Tawk.Mcp.Core;

/// <summary>A file queued for sending. <see cref="Edited"/> is true when the user changed the caption while approving it,
/// and <see cref="Disclaimer"/> when tawk added the user's AI disclaimer under it. <see cref="Text"/> is the caption that went out.</summary>
public sealed record SentFile(
    string Id,
    string File,
    string Type,
    long Bytes,
    bool Edited = false,
    string? Text = null,
    bool Disclaimer = false);
