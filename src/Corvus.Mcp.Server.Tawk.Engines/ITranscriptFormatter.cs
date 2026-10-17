using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Engines;

/// <summary>Turns messages into compact transcript lines a model can read.</summary>
public interface ITranscriptFormatter
{
    string FormatMessages(IEnumerable<ChatMessage> messages, bool includeChat = false);

    string FormatMessage(ChatMessage message, bool includeChat = false);

    string FormatStatuses(IEnumerable<StatusItem> statuses);

    string FormatTimestamp(long unixSeconds);
}
