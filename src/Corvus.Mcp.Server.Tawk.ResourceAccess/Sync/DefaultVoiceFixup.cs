using System.Data.Common;
using Corvus.Mcp.Library.Brokkr.Contracts.Rows;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess.Sync;

/// <summary>Two machines can each have made a voice the default; after a merge the one set last stays the only default.</summary>
public sealed class DefaultVoiceFixup : IApplyFixup
{
    private const string OneDefaultVoice =
        "UPDATE voice SET is_default = 0 WHERE is_default = 1 AND name <> "
        + "(SELECT name FROM voice WHERE is_default = 1 ORDER BY updated DESC, name DESC LIMIT 1)";

    public async Task RunAsync(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(connection);
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = OneDefaultVoice;
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
