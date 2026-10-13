namespace Tawk.Mcp.Core;

/// <summary>Someone played a voice note or video the user sent. <see cref="At"/> is in Unix seconds.</summary>
public sealed record PlayedEvent(ChatRef Chat, string MessageId, ReaderRef Player, long At) : TawkEvent;
