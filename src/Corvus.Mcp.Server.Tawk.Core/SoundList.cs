namespace Corvus.Mcp.Server.Tawk.Core;

/// <summary>The notification sounds a chat can be given, and the one the app plays when a chat has none of its own.</summary>
public sealed record SoundList(IReadOnlyList<SoundEntry> Sounds, string Current);
