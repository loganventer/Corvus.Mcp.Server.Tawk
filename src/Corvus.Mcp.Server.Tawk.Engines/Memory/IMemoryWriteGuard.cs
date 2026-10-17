namespace Corvus.Mcp.Server.Tawk.Engines.Memory;

public interface IMemoryWriteGuard
{
    /// <summary>Throws a MemoryException when memory is read-only.</summary>
    void EnsureWritable();
}
