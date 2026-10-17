using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Managers.Memory;

public interface ICategoryManager
{
    Task<string> ListAsync(CancellationToken cancellationToken);

    Task<string> SetAsync(string path, string? description, CancellationToken cancellationToken);

    Task<string> DeleteAsync(string path, IUserConfirmation? confirmation, CancellationToken cancellationToken);
}
