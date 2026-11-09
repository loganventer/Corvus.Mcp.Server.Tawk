using Corvus.Mcp.Server.Tawk.Core.Transcription;
using Corvus.Mcp.Server.Tawk.ResourceAccess.Transcription;

namespace Corvus.Mcp.Server.Tawk.Tests.ResourceAccess.Transcription;

public class PieceProgressTests
{
    private sealed class Recorder : IProgress<TranscriptionProgress>
    {
        public List<TranscriptionProgress> Seen { get; } = [];

        public void Report(TranscriptionProgress value) => Seen.Add(value);
    }

    [Test]
    public void Half_of_the_second_of_four_even_pieces_is_three_eighths_of_the_note()
    {
        var whole = new Recorder();
        new PieceProgress(whole, 100, 100, 400).Report(new TranscriptionProgress(TranscriptionStage.Transcribing, 50));
        Assert.That(whole.Seen.Single().Percent, Is.EqualTo(37));
    }

    [Test]
    public void A_step_without_a_share_passes_through_unchanged()
    {
        var whole = new Recorder();
        var step = new TranscriptionProgress(TranscriptionStage.LoadingModel);
        new PieceProgress(whole, 100, 100, 400).Report(step);
        Assert.That(whole.Seen.Single(), Is.EqualTo(step));
    }
}
