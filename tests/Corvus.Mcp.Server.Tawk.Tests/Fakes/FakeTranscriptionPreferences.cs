using Corvus.Mcp.Server.Tawk.Core.Transcription;
using Corvus.Mcp.Server.Tawk.ResourceAccess.Transcription;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

/// <summary>The choices in tawk's settings panel, set by the test.</summary>
public sealed class FakeTranscriptionPreferences : ITranscriptionPreferences
{
    public TranscriptionPreferences Chosen { get; set; } = TranscriptionPreferences.None;

    public Task<TranscriptionPreferences> ReadAsync(CancellationToken cancellationToken) => Task.FromResult(Chosen);
}
