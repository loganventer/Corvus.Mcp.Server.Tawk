using Corvus.Mcp.Server.Tawk.Core.Transcription;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess.Transcription;

/// <summary>Reads the user's transcription choices from tawk's settings. corvus-tawk-mcp cannot change them.</summary>
public interface ITranscriptionPreferences
{
    /// <summary>The choices as they stand. It never throws: what tawk does not say comes back null.</summary>
    Task<TranscriptionPreferences> ReadAsync(CancellationToken cancellationToken);
}
