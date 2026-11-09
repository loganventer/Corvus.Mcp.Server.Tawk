using Corvus.Mcp.Server.Tawk.Core.Transcription;

namespace Corvus.Mcp.Server.Tawk.Engines.Transcription;

/// <summary>
/// Cuts a note longer than <see cref="TranscriptionOptions.ChunkSeconds"/> into the fewest even pieces no
/// longer than that, then moves each cut to the quietest moment within a few seconds of it, where a pause
/// between words most likely falls.
/// </summary>
public sealed class QuietPointSplitter(TranscriptionOptions options) : IAudioSplitter
{
    /// <summary>The furthest a cut moves to find a pause.</summary>
    public const double SearchSeconds = 5;

    /// <summary>The stretch whose loudness is measured: long enough to span a short pause, short enough to fit in one.</summary>
    public const double FrameSeconds = 0.2;

    /// <summary>How far apart the measured stretches start.</summary>
    public const double StepSeconds = 0.05;

    public IReadOnlyList<AudioPiece> Split(ReadOnlySpan<float> samples, int sampleRate)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sampleRate);
        var most = (long)options.ChunkSeconds * sampleRate;
        if (samples.Length <= most)
        {
            return [new AudioPiece(0, samples.Length)];
        }

        var count = (int)((samples.Length + most - 1) / most);
        var even = (double)samples.Length / count;

        // A cut never moves far enough to meet the next one.
        var reach = (int)Math.Min(SearchSeconds * sampleRate, even / 4);
        var frame = Math.Max(1, (int)(FrameSeconds * sampleRate));
        var step = Math.Max(1, (int)(StepSeconds * sampleRate));

        var pieces = new List<AudioPiece>(count);
        var start = 0;
        for (var i = 1; i < count; i++)
        {
            var cut = Quietest(samples, (int)(even * i), reach, frame, step);
            pieces.Add(new AudioPiece(start, cut - start));
            start = cut;
        }

        pieces.Add(new AudioPiece(start, samples.Length - start));
        return pieces;
    }

    // The middle of the quietest frame within reach of the even cut.
    private static int Quietest(ReadOnlySpan<float> samples, int around, int reach, int frame, int step)
    {
        var from = Math.Max(0, around - reach);
        var to = Math.Min(samples.Length - frame, around + reach);
        var best = around;
        var lowest = double.MaxValue;
        for (var at = from; at <= to; at += step)
        {
            var energy = 0d;
            foreach (var sample in samples.Slice(at, frame))
            {
                energy += sample * sample;
            }

            if (energy < lowest)
            {
                lowest = energy;
                best = at + frame / 2;
            }
        }

        return best;
    }
}
