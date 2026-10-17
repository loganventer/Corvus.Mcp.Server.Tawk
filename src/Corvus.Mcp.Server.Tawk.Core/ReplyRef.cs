namespace Corvus.Mcp.Server.Tawk.Core;

public sealed record ReplyRef(string Id, string? Sender, string? Text, bool Status);
