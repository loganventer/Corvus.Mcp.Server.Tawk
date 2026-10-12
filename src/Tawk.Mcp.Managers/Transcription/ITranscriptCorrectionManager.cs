namespace Tawk.Mcp.Managers.Transcription;

/// <summary>An agent's corrected text for a voice note, kept in tawk beside the voice note.</summary>
public interface ITranscriptCorrectionManager
{
    Task<string> KeepAsync(string messageId, string language, string text, string? model, CancellationToken cancellationToken);
}
