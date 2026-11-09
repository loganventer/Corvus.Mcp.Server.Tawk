using Corvus.Mcp.Server.Tawk.Core.Transcription;
using Corvus.Mcp.Server.Tawk.Engines.Transcription;

namespace Corvus.Mcp.Server.Tawk.Tests.Engines.Transcription;

public class TranscriptJoinerTests
{
    private readonly TranscriptJoiner _joiner = new();

    [Test]
    public void Pieces_are_joined_in_order_with_the_first_pieces_language()
    {
        var joined = _joiner.Join(
        [
            new Transcript(" Hallo, Blom. ", "af", "large-v3", 120),
            new Transcript("", null, "large-v3", 2),
            new Transcript("Love you, bye.", null, "large-v3", 98.5),
        ]);
        Assert.That(joined, Is.EqualTo(new Transcript("Hallo, Blom. Love you, bye.", "af", "large-v3", 220.5)));
    }

    [Test]
    public void The_length_is_unknown_when_a_piece_does_not_say()
    {
        var joined = _joiner.Join([new Transcript("a", null, "m", 10), new Transcript("b", null, "m", null)]);
        Assert.That(joined.DurationS, Is.Null);
    }
}
