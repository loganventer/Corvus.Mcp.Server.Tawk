using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Managers;

public interface IChatManagementManager
{
    Task<string> SetChatAsync(string chat, string? muted, bool? pinned, bool? archived, bool? locked, WriteContext context, CancellationToken cancellationToken);

    Task<string> SetChatThemeAsync(string chat, string theme, WriteContext context, CancellationToken cancellationToken);

    Task<string> ClearChatAsync(string chat, WriteContext context, CancellationToken cancellationToken);

    Task<string> DeleteChatAsync(string chat, WriteContext context, CancellationToken cancellationToken);

    Task<string> ExportChatAsync(string chat, bool withMedia, WriteContext context, CancellationToken cancellationToken);

    Task<string> BlockAsync(string chat, WriteContext context, CancellationToken cancellationToken);

    Task<string> UnblockAsync(string chat, WriteContext context, CancellationToken cancellationToken);
}
