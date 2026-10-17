using Corvus.Mcp.Server.Tawk.Core.Sync;

namespace Corvus.Mcp.Server.Tawk.Engines.Sync;

/// <summary>A fingerprint of what a memory database holds, the same for the same rows whatever the file's bytes.</summary>
public interface IMemoryDigest
{
    string Compute(MemorySnapshot snapshot);
}
