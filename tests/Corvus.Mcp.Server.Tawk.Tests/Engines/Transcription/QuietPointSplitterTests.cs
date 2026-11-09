using Corvus.Mcp.Server.Tawk.Core.Transcription;
using Corvus.Mcp.Server.Tawk.Engines.Transcription;

namespace Corvus.Mcp.Server.Tawk.Tests.Engines.Transcription;

public class QuietPointSplitterTests
{
    private const int Rate = 100;

    private static QuietPointSplitter Splitter(int chunkSeconds) => new(new TranscriptionOptions { ChunkSeconds = chunkSeconds });

    private static float[] Loud(int seconds) => Enumerable.Repeat(0.5f, seconds * Rate).ToArray();

    [Test]
    public void A_note_no_longer_than_a_piece_is_heard_whole()
    {
        var pieces = Splitter(120).Split(Loud(120), Rate);
        Assert.That(pieces, Is.EqualTo(new[] { new AudioPiece(0, 120 * Rate) }));
    }

    [Test]
    public void A_long_note_is_cut_into_the_fewest_pieces_that_cover_every_sample_once()
    {
        var samples = Loud(618);
        var pieces = Splitter(120).Split(samples, Rate);
        Assert.Multiple(() =>
        {
            Assert.That(pieces, Has.Count.EqualTo(6));
            Assert.That(pieces[0].Start, Is.Zero);
            for (var i = 1; i < pieces.Count; i++)
            {
                Assert.That(pieces[i].Start, Is.EqualTo(pieces[i - 1].Start + pieces[i - 1].Length));
            }

            Assert.That(pieces[^1].Start + pieces[^1].Length, Is.EqualTo(samples.Length));
            Assert.That(pieces.Max(p => p.Length), Is.LessThanOrEqualTo((120 + QuietPointSplitter.SearchSeconds) * Rate));
        });
    }

    [Test]
    public void A_cut_moves_to_the_pause_nearest_the_even_cut()
    {
        // 200 s splits evenly at 100 s; the speaker pauses at 103 s.
        var samples = Loud(200);
        Array.Fill(samples, 0f, 103 * Rate, Rate / 2);
        var pieces = Splitter(120).Split(samples, Rate);
        Assert.That(pieces[1].Start, Is.InRange(103 * Rate, 103 * Rate + Rate / 2));
    }

    [Test]
    public void A_pause_further_away_than_the_search_is_not_used()
    {
        var samples = Loud(200);
        Array.Fill(samples, 0f, 130 * Rate, Rate);
        var pieces = Splitter(120).Split(samples, Rate);
        Assert.That(pieces[1].Start, Is.InRange((100 - QuietPointSplitter.SearchSeconds) * Rate, (100 + QuietPointSplitter.SearchSeconds) * Rate));
    }
}
