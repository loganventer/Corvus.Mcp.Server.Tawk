using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Engines;

/// <summary>Says in words what happened to a file the user approved, for the model that asked.</summary>
public interface ISentFileDescriber
{
    string Describe(SentFile sent);
}
