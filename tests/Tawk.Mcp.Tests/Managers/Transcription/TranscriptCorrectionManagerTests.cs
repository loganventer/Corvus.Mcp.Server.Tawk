using Tawk.Mcp.Managers.Transcription;
using Tawk.Mcp.ResourceAccess.Transcription;

namespace Tawk.Mcp.Tests.Managers.Transcription;

public class TranscriptCorrectionManagerTests
{
    private sealed class RecordingKeeper : ITranscriptKeeper
    {
        public List<(string MessageId, string Language, string Text, string? Model)> Kept { get; } = [];

        public Task KeepAsync(string messageId, string language, string text, string? model, CancellationToken cancellationToken)
        {
            Kept.Add((messageId, language, text, model));
            return Task.CompletedTask;
        }
    }

    private readonly RecordingKeeper _keeper = new();

    [Test]
    public async Task The_corrected_text_is_kept_trimmed_and_the_answer_says_so()
    {
        var said = await new TranscriptCorrectionManager(_keeper).KeepAsync(" 3EB0 ", " af ", "  Hallo daar.  ", "sonnet", CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(said, Is.EqualTo(TranscriptCorrectionManager.Kept));
            Assert.That(_keeper.Kept, Is.EqualTo(new[] { ("3EB0", "af", "Hallo daar.", (string?)"sonnet") }));
        });
    }

    [TestCase("", "af", "text")]
    [TestCase("3EB0", "", "text")]
    [TestCase("3EB0", "af", "   ")]
    public void A_missing_part_is_refused_before_tawk_hears_of_it(string messageId, string language, string text)
    {
        var manager = new TranscriptCorrectionManager(_keeper);

        Assert.ThrowsAsync<ArgumentException>(() => manager.KeepAsync(messageId, language, text, null, CancellationToken.None));
        Assert.That(_keeper.Kept, Is.Empty);
    }
}
