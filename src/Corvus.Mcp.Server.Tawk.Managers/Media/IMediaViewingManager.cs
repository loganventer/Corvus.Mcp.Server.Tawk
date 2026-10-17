using Corvus.Mcp.Server.Tawk.Core.Media;

namespace Corvus.Mcp.Server.Tawk.Managers.Media;

public interface IMediaViewingManager
{
    /// <summary>
    /// The picture of a message, for the client's own model to look at. Throws
    /// <see cref="MediaException"/> when there is none to show or tawk is still fetching it.
    /// </summary>
    Task<ImageData> ViewImageAsync(string messageId, CancellationToken cancellationToken);
}
