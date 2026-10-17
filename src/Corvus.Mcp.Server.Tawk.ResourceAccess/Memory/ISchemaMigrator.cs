using Microsoft.Data.Sqlite;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess.Memory;

/// <summary>Brings a database up to the current schema. Running it again changes nothing.</summary>
public interface ISchemaMigrator
{
    Task MigrateAsync(SqliteConnection connection, CancellationToken cancellationToken);
}
