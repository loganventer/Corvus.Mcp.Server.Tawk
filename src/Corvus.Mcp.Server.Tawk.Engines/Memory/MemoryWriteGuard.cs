using Corvus.Mcp.Server.Tawk.Core.Memory;

namespace Corvus.Mcp.Server.Tawk.Engines.Memory;

public sealed class MemoryWriteGuard(MemoryMode mode) : IMemoryWriteGuard
{
    public void EnsureWritable()
    {
        if (mode != MemoryMode.Write)
        {
            throw new MemoryException("corvus-tawk-mcp's memory is read-only (--memory read), so nothing was changed.");
        }
    }
}
