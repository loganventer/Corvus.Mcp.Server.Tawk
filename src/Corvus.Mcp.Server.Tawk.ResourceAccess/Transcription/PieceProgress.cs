using Corvus.Mcp.Server.Tawk.Core.Transcription;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess.Transcription;

/// <summary>
/// Reports one piece's progress as progress through the whole note: a piece that is half heard counts for
/// half its share of the note, on top of the pieces before it.
/// </summary>
public sealed class PieceProgress(IProgress<TranscriptionProgress> whole, int before, int length, int total) : IProgress<TranscriptionProgress>
{
    public void Report(TranscriptionProgress value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.Percent is not { } percent || total <= 0)
        {
            whole.Report(value);
            return;
        }

        var heard = before + (long)length * percent / 100;
        whole.Report(value with { Percent = (int)(heard * 100 / total) });
    }
}
