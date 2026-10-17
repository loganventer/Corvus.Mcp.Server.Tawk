using Corvus.Mcp.Server.Tawk.Core.Transcription;
using Corvus.Mcp.Server.Tawk.ResourceAccess.Transcription;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

/// <summary>Counts how often its model was let go, and transcribes nothing.</summary>
public sealed class FakeWhisperModelHost : IWhisperModelHost
{
    public int Unloads { get; private set; }

    public Task<Transcript> TranscribeAsync(
        ReadOnlyMemory<float> samples, TranscriptionPassRequest request, IProgress<TranscriptionProgress> progress, CancellationToken cancellationToken) =>
        Task.FromResult(new Transcript("from the model inside", null, request.Model, samples.Length / 16000.0));

    public Task UnloadAsync(CancellationToken cancellationToken)
    {
        Unloads++;
        return Task.CompletedTask;
    }
}
