using Corvus.Mcp.Server.Tawk.Core.Sync;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess.Sync;

/// <summary>Reads what syncs out of a memory database, and writes a merge back into the local one.</summary>
public interface IMemorySnapshotStore
{
    Task<MemorySnapshot> ReadAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Reads a memory database from a file, such as a downloaded copy. One from an older corvus-tawk-mcp is brought
    /// up to this schema first; one from a newer corvus-tawk-mcp is refused with a MemoryException.
    /// </summary>
    Task<MemorySnapshot> ReadFileAsync(string path, CancellationToken cancellationToken);

    /// <summary>Applies a merge in one transaction: all of it or none.</summary>
    Task ApplyAsync(MergePlan plan, CancellationToken cancellationToken);

    /// <summary>Writes a consistent copy of the local database to a file that does not exist yet.</summary>
    Task BackupAsync(string destination, CancellationToken cancellationToken);
}
