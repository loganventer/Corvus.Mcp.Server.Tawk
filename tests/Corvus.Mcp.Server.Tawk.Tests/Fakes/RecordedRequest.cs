using System.Text.Json.Nodes;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

public sealed record RecordedRequest(string Op, JsonObject? Args);
