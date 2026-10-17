using System.ComponentModel;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.Managers.Transcription;

namespace Corvus.Mcp.Server.Tawk.Clients.Tools;

[McpServerToolType]
public sealed class TranscriptCorrectionTools(ITranscriptCorrectionManager corrections, IAccountScope accounts)
{
    [McpServerTool(Name = "set_transcript", Destructive = false, ReadOnly = false, Idempotent = true, OpenWorld = false)]
    [Description(
        "Keep your corrected text of a voice note in tawk, in place of the transcript tawk holds for that language, so the user reads the "
        + "corrected words under the voice note. Use it after you have cleaned a transcript as its note says: fix what speech recognition "
        + "clearly got wrong and stay as close as you can to what was said. Give the message id and the language the speaker used, as the "
        + "transcript event shows them. Only the transcript for that language is replaced; the others stay. It sends nothing to WhatsApp and "
        + "needs no approval, and tawk refuses it for a chat that is not transcribed, a message that is not audio, or text that is too long. "
        + "Never put anything in it that the speaker did not say.")]
    public Task<CallToolResult> SetTranscriptAsync(
        [Description("The id of the voice note, from the transcript event.")] string messageId,
        [Description("The language the text is in, as a short code such as af or en: the language of the pass you corrected.")] string language,
        [Description("The corrected text of the whole voice note, as plain text.")] string text,
        [Description("The name of the model that corrected it, when you know it.")] string? model = null,
        [Description(ToolText.Account)] string? account = null,
        CancellationToken cancellationToken = default) =>
        ToolResults.RunAsync(accounts, account, () => corrections.KeepAsync(messageId, language, text, model, cancellationToken));
}
