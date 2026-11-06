using System.Text.Json.Nodes;
using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.Engines;
using Corvus.Mcp.Server.Tawk.ResourceAccess;

namespace Corvus.Mcp.Server.Tawk.Managers;

public sealed class ChatToneManager(ITawkControl control, IConfirmationGate gate, ISoundListFormatter formatter) : IChatToneManager
{
    private const string OlderTawk = "This tawk cannot give a chat its own sound from outside (it needs tawk 0.36.0 or later).";

    public async Task<string> ListSoundsAsync(CancellationToken cancellationToken)
    {
        await NeedAsync(cancellationToken).ConfigureAwait(false);
        var list = await control.RequestAsync<SoundList>("list_sounds", null, cancellationToken).ConfigureAwait(false);
        return formatter.Format(list);
    }

    public async Task<string> SetChatToneAsync(string chat, string tone, WriteContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tone);
        ArgumentNullException.ThrowIfNull(context);
        await NeedAsync(cancellationToken).ConfigureAwait(false);
        var wanted = tone.Trim();
        var outcome = await gate.ExecuteAsync(
            "set_chat_tone",
            new JsonObject { ["chat"] = chat, ["tone"] = wanted },
            context.Confirmation,
            context.OnApprovalWaiting,
            cancellationToken).ConfigureAwait(false);
        return WriteResults.Describe(outcome, _ => wanted switch
        {
            ChatTones.AppSound => "The chat now plays the app's sound.",
            ChatTones.Silent => "The chat is silent now.",
            _ => $"The chat now plays {wanted}.",
        });
    }

    private async Task NeedAsync(CancellationToken cancellationToken)
    {
        var hello = await control.ConnectAsync(cancellationToken).ConfigureAwait(false);
        if (!hello.Has(TawkFeatures.ChatTones))
        {
            throw new TawkControlException(OlderTawk);
        }
    }
}
