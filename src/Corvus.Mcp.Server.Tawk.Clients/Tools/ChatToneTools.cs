using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.Managers;

namespace Corvus.Mcp.Server.Tawk.Clients.Tools;

[McpServerToolType]
public sealed class ChatToneTools(IChatToneManager tones, IAccountScope accounts)
{
    [McpServerTool(Name = "list_sounds", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("List the notification sounds in tawk's sound folders, for set_chat_tone, and the one the app plays.")]
    public Task<CallToolResult> ListSoundsAsync(CancellationToken cancellationToken = default) =>
        ToolResults.RunAsync(() => tones.ListSoundsAsync(cancellationToken));

    [McpServerTool(Name = "set_chat_tone", Destructive = false, ReadOnly = false, Idempotent = true, OpenWorld = false)]
    [Description("Give a contact or group its own notification sound: a sound from list_sounds, none for silence, or an empty tone for the app's sound. "
        + "Only a sound's name is accepted, never a file." + ToolText.NeedsManage)]
    public Task<CallToolResult> SetChatToneAsync(
        McpServer? server,
        [Description("The chat's jid or name.")] string chat,
        [Description("A sound id from list_sounds, none, or an empty string.")] string tone,
        [Description(ToolText.Account)] string? account = null,
        IProgress<ProgressNotificationValue>? progress = null,
        CancellationToken cancellationToken = default) =>
        ToolResults.RunAsync(accounts, account, () => tones.SetChatToneAsync(chat, tone, WriteContexts.For(server, progress), cancellationToken));
}
