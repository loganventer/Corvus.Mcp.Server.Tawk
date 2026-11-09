using Corvus.Mcp.Server.Tawk.Core.Transcription;
using Corvus.Mcp.Server.Tawk.Engines.Transcription;
using Corvus.Mcp.Server.Tawk.ResourceAccess.Transcription;

namespace Corvus.Mcp.Server.Tawk.Tests.ResourceAccess.Transcription;

public class EmbeddedTranscriberTests
{
    private sealed class SilentDecoder(int seconds) : IAudioDecoder
    {
        public float[] Decode(string path, int maxSeconds) => new float[seconds * OggOpusDecoder.SampleRate];
    }

    // Hears each piece as its length, and Afrikaans when left to detect.
    private sealed class RecordingHost : IWhisperModelHost
    {
        public List<(int Samples, string Language)> Calls { get; } = [];

        public Task<Transcript> TranscribeAsync(
            ReadOnlyMemory<float> samples, TranscriptionPassRequest request, IProgress<TranscriptionProgress> progress, CancellationToken cancellationToken)
        {
            Calls.Add((samples.Length, request.Language));
            var seconds = samples.Length / (double)OggOpusDecoder.SampleRate;
            return Task.FromResult(new Transcript($"{seconds:0}s", request.LanguageHint is null ? "af" : null, request.Model, seconds));
        }

        public Task UnloadAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private static (EmbeddedTranscriber Transcriber, RecordingHost Host) Make(int noteSeconds)
    {
        var options = new TranscriptionOptions { ChunkSeconds = 120 };
        var host = new RecordingHost();
        return (new EmbeddedTranscriber(new SilentDecoder(noteSeconds), host, new QuietPointSplitter(options), new TranscriptJoiner(), options), host);
    }

    private static TranscriptionPassRequest Auto() => new("note.ogg", TranscriptionOptions.Auto, TranscriptionTask.Transcribe, "large-v3", null);

    [Test]
    public async Task A_short_note_goes_to_the_model_once()
    {
        var (transcriber, host) = Make(47);
        var transcript = await transcriber.TranscribeAsync(Auto(), new Progress<TranscriptionProgress>(), CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(host.Calls, Has.Count.EqualTo(1));
            Assert.That(transcript.Text, Is.EqualTo("47s"));
        });
    }

    [Test]
    public async Task A_long_note_is_heard_in_pieces_and_keeps_the_language_of_the_first()
    {
        var (transcriber, host) = Make(618);
        var transcript = await transcriber.TranscribeAsync(Auto(), new Progress<TranscriptionProgress>(), CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(host.Calls, Has.Count.EqualTo(6));
            Assert.That(host.Calls[0].Language, Is.EqualTo(TranscriptionOptions.Auto));
            Assert.That(host.Calls.Skip(1).Select(c => c.Language), Is.All.EqualTo("af"));
            Assert.That(host.Calls.Sum(c => c.Samples), Is.EqualTo(618 * OggOpusDecoder.SampleRate));
            Assert.That(transcript.DetectedLanguage, Is.EqualTo("af"));
            Assert.That(transcript.DurationS, Is.EqualTo(618).Within(0.01));
        });
    }
}
