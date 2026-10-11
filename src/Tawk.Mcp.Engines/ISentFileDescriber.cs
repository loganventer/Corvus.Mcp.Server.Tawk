using Tawk.Mcp.Core;

namespace Tawk.Mcp.Engines;

/// <summary>Says in words what happened to a file the user approved, for the model that asked.</summary>
public interface ISentFileDescriber
{
    string Describe(SentFile sent);
}
