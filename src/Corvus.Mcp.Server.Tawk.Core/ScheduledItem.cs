namespace Corvus.Mcp.Server.Tawk.Core;

public sealed record ScheduledItem(string Id, string Chat, string Text, long DueAt);
