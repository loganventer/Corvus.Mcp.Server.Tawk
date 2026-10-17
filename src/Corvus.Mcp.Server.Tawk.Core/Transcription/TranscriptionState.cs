namespace Corvus.Mcp.Server.Tawk.Core.Transcription;

public enum TranscriptionState
{
    Queued,

    Running,

    /// <summary>Every pass produced text.</summary>
    Done,

    /// <summary>Some passes produced text and some failed.</summary>
    Partial,

    Failed,
}
