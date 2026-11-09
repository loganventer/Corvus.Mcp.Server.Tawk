namespace Corvus.Mcp.Server.Tawk.Core.Transcription;

/// <summary>One stretch of a voice note's samples: where it starts and how many samples it holds.</summary>
public readonly record struct AudioPiece(int Start, int Length);
