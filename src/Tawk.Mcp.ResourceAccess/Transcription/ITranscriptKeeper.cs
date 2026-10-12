namespace Tawk.Mcp.ResourceAccess.Transcription;

/// <summary>
/// Keeps text an agent wrote for a voice note in tawk, in place of what tawk kept for that language.
/// Unlike <see cref="ITranscriptHandoff"/>, which quietly passes over what tawk will not take, a refusal
/// is raised, because an agent asked for it and has to be told.
/// </summary>
public interface ITranscriptKeeper
{
    /// <exception cref="Tawk.Mcp.Core.TawkControlException">tawk is too old to keep transcripts, or refused this one.</exception>
    Task KeepAsync(string messageId, string language, string text, string? model, CancellationToken cancellationToken);
}
