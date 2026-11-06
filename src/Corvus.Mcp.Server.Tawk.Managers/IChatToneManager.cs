using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Managers;

/// <summary>The notification sound each chat plays: the sounds to choose from, and giving a chat one.</summary>
public interface IChatToneManager
{
    Task<string> ListSoundsAsync(CancellationToken cancellationToken);

    /// <summary>Gives a chat a sound by its id, <see cref="ChatTones.Silent"/>, or <see cref="ChatTones.AppSound"/>.</summary>
    Task<string> SetChatToneAsync(string chat, string tone, WriteContext context, CancellationToken cancellationToken);
}
