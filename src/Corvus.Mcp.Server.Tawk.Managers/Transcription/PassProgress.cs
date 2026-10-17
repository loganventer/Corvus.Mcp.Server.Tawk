using Corvus.Mcp.Server.Tawk.Core.Transcription;
using Corvus.Mcp.Server.Tawk.ResourceAccess.Transcription;

namespace Corvus.Mcp.Server.Tawk.Managers.Transcription;

/// <summary>Carries what one pass reports into the job store, naming the language the pass is for.</summary>
public sealed class PassProgress(ITranscriptionJobStore jobs, string jobId, string language) : IProgress<TranscriptionProgress>
{
    public void Report(TranscriptionProgress value)
    {
        ArgumentNullException.ThrowIfNull(value);
        jobs.Report(jobId, value with { Language = language });
    }
}
