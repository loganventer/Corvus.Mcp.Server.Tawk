namespace Corvus.Mcp.Server.Tawk.Host;

/// <summary>A token passed in from outside, such as a container secret in CORVUS_TAWK_MCP_TOKEN.</summary>
public sealed class FixedTokenStore(string token) : ITokenStore
{
    public string GetOrCreate() => token;

    public string Describe() => "CORVUS_TAWK_MCP_TOKEN";
}
