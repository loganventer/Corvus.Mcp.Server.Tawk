namespace Corvus.Mcp.Server.Tawk.Core.Transcription;

/// <summary>Puts the transcripts of a voice note's pieces back together as one.</summary>
public interface ITranscriptJoiner
{
    /// <summary>The pieces' transcripts, in the order they were spoken, as one transcript of the whole note.</summary>
    Transcript Join(IReadOnlyList<Transcript> pieces);
}
