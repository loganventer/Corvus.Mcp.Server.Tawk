using Corvus.Mcp.Server.Tawk.Core.Transcription;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess.Transcription;

/// <summary>Transcribes inside corvus-tawk-mcp: decodes the voice note and hands the samples to the one loaded model.</summary>
public sealed class EmbeddedTranscriber(IAudioDecoder decoder, IWhisperModelHost host, TranscriptionOptions options) : ITranscriber
{
    public string Name => "embedded";

    public async Task<Transcript> TranscribeAsync(TranscriptionPassRequest request, IProgress<TranscriptionProgress> progress, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(progress);
        progress.Report(new TranscriptionProgress(TranscriptionStage.DecodingAudio));
        var samples = await Task.Run(() => decoder.Decode(request.Path, options.MaxSeconds), cancellationToken).ConfigureAwait(false);
        return await host.TranscribeAsync(samples, request, progress, cancellationToken).ConfigureAwait(false);
    }
}
