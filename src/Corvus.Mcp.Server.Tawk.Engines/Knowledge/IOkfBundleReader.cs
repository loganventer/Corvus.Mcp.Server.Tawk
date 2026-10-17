using Corvus.Mcp.Server.Tawk.Core.Okf;

namespace Corvus.Mcp.Server.Tawk.Engines.Knowledge;

/// <summary>Reads an Open Knowledge Format bundle, written by corvus-tawk-mcp or by anything else that follows the format.</summary>
public interface IOkfBundleReader
{
    /// <summary><paramref name="now"/> dates whatever the files do not date themselves.</summary>
    OkfBundleReading Read(IReadOnlyList<OkfBundleFile> files, DateTimeOffset now);
}
