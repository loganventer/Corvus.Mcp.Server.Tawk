using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Managers;

public interface IProfileManager
{
    Task<string> GetProfileAsync(CancellationToken cancellationToken);

    Task<string> SetProfileAsync(string? name, string? about, WriteContext context, CancellationToken cancellationToken);

    Task<string> SetProfilePhotoAsync(string file, WriteContext context, CancellationToken cancellationToken);

    Task<string> RemoveProfilePhotoAsync(WriteContext context, CancellationToken cancellationToken);
}
