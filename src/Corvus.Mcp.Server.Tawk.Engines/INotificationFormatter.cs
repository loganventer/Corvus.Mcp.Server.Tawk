using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Engines;

/// <summary>Writes the short header line announcing a new message to an agent.</summary>
public interface INotificationFormatter
{
    string Header(ChatRef chat, ChatMessage message);

    /// <summary>One line saying who read which of the user's messages.</summary>
    string Read(ChatRef chat, string messageId, ReaderRef reader);

    /// <summary>One line saying who played which of the user's voice notes or videos, and the exact time (date and seconds) they did.</summary>
    string Played(ChatRef chat, string messageId, ReaderRef player, long at);

    /// <summary>One line saying what happened to a message: a reaction, an edit, a delete or a scheduled send.</summary>
    string Activity(MessageActivityEvent activity);

    /// <summary>One line saying that the person in a chat came online or left, and when they were last seen where that is known.</summary>
    string Presence(PresenceEvent presence);

    /// <summary>The line that says which of the user's accounts an event is about. The label is the user's own text and is flattened like a name.</summary>
    string Account(AccountRef account);
}
