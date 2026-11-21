using System.Data.Common;
using Corvus.Mcp.Library.Brokkr.Contracts.Rows;
using Corvus.Mcp.Server.Tawk.ResourceAccess.Memory;
using Microsoft.Data.Sqlite;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess.Sync;

/// <summary>
/// Opens memory databases for sync: the live one through the server's own connection factory (migrated, WAL, foreign keys on),
/// and a downloaded copy as a plain file that is let go entirely when closed. Memory is plain SQLite, never encrypted.
/// </summary>
public sealed class TawkSqliteOpener(ISqliteConnectionFactory live, ISchemaMigrator migrator) : ISqliteOpener
{
    public int SchemaVersion => SqliteSchemaMigrator.CurrentVersion;

    public async Task<DbConnection> OpenAsync(string path, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        if (Path.GetFullPath(path) == Path.GetFullPath(live.Path))
        {
            return await live.OpenAsync(cancellationToken).ConfigureAwait(false);
        }

        var copy = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path, Pooling = false }.ToString());
        await copy.OpenAsync(cancellationToken).ConfigureAwait(false);
        return copy;
    }

    public bool IsUnencrypted(string path) => false;

    public Task UpgradeAsync(DbConnection connection, CancellationToken cancellationToken) =>
        migrator.MigrateAsync((SqliteConnection)connection, cancellationToken);
}
