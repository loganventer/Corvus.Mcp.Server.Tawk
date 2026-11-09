using Corvus.Mcp.Server.Tawk.Core.Transcription;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess.Transcription;

/// <summary>
/// Transcribes inside corvus-tawk-mcp: decodes the voice note, cuts a long one into pieces, and hands each
/// piece to the one loaded model in turn. Each piece reports its own progress, so a long note never looks
/// stuck, and a piece the model loops on spoils only that piece.
/// </summary>
public sealed class EmbeddedTranscriber(
    IAudioDecoder decoder, IWhisperModelHost host, IAudioSplitter splitter, ITranscriptJoiner joiner, TranscriptionOptions options) : ITranscriber
{
    public string Name => "embedded";

    public async Task<Transcript> TranscribeAsync(TranscriptionPassRequest request, IProgress<TranscriptionProgress> progress, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(progress);
        progress.Report(new TranscriptionProgress(TranscriptionStage.DecodingAudio));
        var samples = await Task.Run(() => decoder.Decode(request.Path, options.MaxSeconds), cancellationToken).ConfigureAwait(false);
        var pieces = splitter.Split(samples, OggOpusDecoder.SampleRate);

        var transcripts = new List<Transcript>(pieces.Count);
        var pass = request;
        foreach (var piece in pieces)
        {
            var heard = await host.TranscribeAsync(
                    samples.AsMemory(piece.Start, piece.Length),
                    pass,
                    new PieceProgress(progress, piece.Start, piece.Length, samples.Length),
                    cancellationToken)
                .ConfigureAwait(false);
            transcripts.Add(heard);

            // The language heard in the first piece holds for the rest, so the note is not written in two.
            if (pass.LanguageHint is null && heard.DetectedLanguage is { } language)
            {
                pass = pass with { Language = language };
            }
        }

        return joiner.Join(transcripts);
    }
}
