namespace Corvus.Mcp.Server.Tawk.Core.Transcription;

/// <summary>Cuts a long voice note into pieces a model can finish one at a time.</summary>
public interface IAudioSplitter
{
    /// <summary>
    /// The pieces, in order, covering every sample once. A note no longer than the configured piece is one
    /// piece; a longer one is cut where the speaker is quietest near each even cut, so words are not split.
    /// </summary>
    IReadOnlyList<AudioPiece> Split(ReadOnlySpan<float> samples, int sampleRate);
}
