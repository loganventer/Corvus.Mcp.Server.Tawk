using Tawk.Mcp.Core;

namespace Tawk.Mcp.Engines;

/// <summary>Words for what is known about a voice note or video: how long it is and when it was played.</summary>
public interface IAudioDetailFormatter
{
    /// <summary>The length as "0:42" or "1:05:09", or null when it is not known.</summary>
    string? Length(ChatMessage message);

    /// <summary>"played 2026-10-13 13:58:42": always the date and the exact time, however old the message is, or null when it is not known to be played.</summary>
    string? Played(ChatMessage message);
}
