using Corvus.Mcp.Server.Tawk.ResourceAccess.Transcription;

namespace Corvus.Mcp.Server.Tawk.Managers.Transcription;

public sealed class TranscriptCorrectionManager(ITranscriptKeeper keeper) : ITranscriptCorrectionManager
{
    public const string Kept = "tawk kept the corrected transcript in place of the one it had for that language.";

    public async Task<string> KeepAsync(string messageId, string language, string text, string? model, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageId);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        await keeper.KeepAsync(messageId.Trim(), language.Trim(), text.Trim(), model, cancellationToken).ConfigureAwait(false);
        return Kept;
    }
}
