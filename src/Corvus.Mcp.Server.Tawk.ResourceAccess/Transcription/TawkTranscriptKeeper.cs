using System.Text.Json.Nodes;
using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess.Transcription;

public sealed class TawkTranscriptKeeper(ITawkControl control) : ITranscriptKeeper
{
    public const string OlderTawk = "This tawk keeps no voice note transcripts (it needs tawk 0.13.0 or later).";

    public async Task KeepAsync(string messageId, string language, string text, string? model, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(messageId);
        ArgumentException.ThrowIfNullOrEmpty(language);
        ArgumentException.ThrowIfNullOrEmpty(text);
        var hello = await control.ConnectAsync(cancellationToken).ConfigureAwait(false);
        if (!hello.Has(TawkFeatures.Transcripts))
        {
            throw new TawkControlException(OlderTawk);
        }

        // Without "replace", tawk puts this in the place of the transcript it holds for that language only.
        var args = new JsonObject
        {
            ["message_id"] = messageId,
            ["language"] = language,
            ["text"] = text,
            ["model"] = model ?? string.Empty,
        };
        await control.RequestAsync("set_transcript", args, cancellationToken).ConfigureAwait(false);
    }
}
