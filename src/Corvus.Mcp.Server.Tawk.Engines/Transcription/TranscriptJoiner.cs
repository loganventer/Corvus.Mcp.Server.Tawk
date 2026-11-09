using Corvus.Mcp.Server.Tawk.Core.Transcription;

namespace Corvus.Mcp.Server.Tawk.Engines.Transcription;

/// <summary>
/// Joins pieces' text with a space, in order. The language heard is the first piece's, the one the later
/// pieces were told to keep to, and the length is the sum of the pieces' lengths when every piece reports one.
/// </summary>
public sealed class TranscriptJoiner : ITranscriptJoiner
{
    public Transcript Join(IReadOnlyList<Transcript> pieces)
    {
        ArgumentNullException.ThrowIfNull(pieces);
        ArgumentOutOfRangeException.ThrowIfZero(pieces.Count);
        var text = string.Join(' ', pieces.Select(p => p.Text.Trim()).Where(t => t.Length > 0));
        double? seconds = pieces.All(p => p.DurationS is not null) ? pieces.Sum(p => p.DurationS!.Value) : null;
        return new Transcript(text, pieces[0].DetectedLanguage, pieces[0].Model, seconds);
    }
}
