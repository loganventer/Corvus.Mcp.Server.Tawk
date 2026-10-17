namespace Corvus.Mcp.Server.Tawk.Core.Okf;

/// <summary>This running corvus-tawk-mcp as an OKF actor, for what it writes on its own account.</summary>
public sealed record OkfProducer(string Version)
{
    public string Actor => OkfActors.Tool(Version);
}
