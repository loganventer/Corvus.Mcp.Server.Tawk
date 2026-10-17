using Corvus.Mcp.Server.Tawk.Core.Okf;

namespace Corvus.Mcp.Server.Tawk.Engines.Knowledge;

/// <summary>Writes knowledge as an Open Knowledge Format 0.2 bundle: one Markdown file per concept and an index.</summary>
public interface IOkfBundleWriter
{
    OkfBundleWriting Write(OkfBundle bundle);
}
