using System.Text.Json;

namespace Corvus.Mcp.Server.Tawk.Managers;

internal static class JsonDefaults
{
    public static JsonSerializerOptions Options { get; } = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
}
