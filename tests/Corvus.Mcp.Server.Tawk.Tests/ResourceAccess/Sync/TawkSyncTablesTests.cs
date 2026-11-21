using Corvus.Mcp.Server.Tawk.ResourceAccess.Memory;
using Corvus.Mcp.Server.Tawk.ResourceAccess.Sync;
using Corvus.Mcp.Server.Tawk.Tests.Fakes;

namespace Corvus.Mcp.Server.Tawk.Tests.ResourceAccess.Sync;

/// <summary>The synced tables against the real schema, so a column renamed in a migration cannot silently drop out of sync.</summary>
public sealed class TawkSyncTablesTests
{
    [Test]
    public async Task Every_synced_column_key_and_parent_exists_in_the_schema()
    {
        using var parts = new MemoryParts();
        var connection = await parts.Connections.OpenAsync(CancellationToken.None);
        await using (connection)
        {
            foreach (var table in TawkSyncTables.All)
            {
                var columns = await ColumnsAsync(connection, table.Name);
                Assert.Multiple(() =>
                {
                    Assert.That(columns, Is.SupersetOf(table.Columns), table.Name);
                    Assert.That(table.Columns, Is.SupersetOf(table.KeyColumns.Append(table.UpdatedColumn)), table.Name);
                });
                if (table.Parent is { } parent)
                {
                    Assert.That(await ColumnsAsync(connection, parent.Table), Does.Contain(parent.KeyColumn), table.Name);
                }

                // The rank and expiry expressions must run against the table as it is.
                using var probe = connection.CreateCommand();
                probe.CommandText = $"SELECT {table.RankSql ?? "NULL"} FROM {table.Name} LIMIT 1";
                await probe.ExecuteNonQueryAsync();
            }
        }
    }

    [Test]
    public void Every_table_names_the_tawk_rule_and_parents_come_first()
    {
        var seen = new HashSet<string>();
        foreach (var table in TawkSyncTables.All)
        {
            Assert.That(table.RuleName, Is.EqualTo(TawkSyncTables.Rule));
            if (table.Parent is { } parent)
            {
                Assert.That(seen, Does.Contain(parent.Table), table.Name + " is listed before its parent");
            }

            seen.Add(table.Name);
        }
    }

    private static async Task<List<string>> ColumnsAsync(Microsoft.Data.Sqlite.SqliteConnection connection, string table)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT name FROM pragma_table_info('{table}')";
        var columns = new List<string>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            columns.Add(reader.GetString(0));
        }

        return columns;
    }
}
