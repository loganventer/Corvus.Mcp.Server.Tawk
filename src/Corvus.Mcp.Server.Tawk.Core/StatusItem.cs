namespace Corvus.Mcp.Server.Tawk.Core;

public sealed record StatusItem(
    string Id,
    string? Author,
    string? AuthorName,
    bool FromMe,
    string Type,
    string? Text,
    long Ts,
    bool Viewed);
