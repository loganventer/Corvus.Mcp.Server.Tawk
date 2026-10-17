using Corvus.Mcp.Server.Tawk.Core.Memory;
using Corvus.Mcp.Server.Tawk.Engines.Memory;
using Corvus.Mcp.Server.Tawk.ResourceAccess.Memory;

namespace Corvus.Mcp.Server.Tawk.Managers.Memory;

public sealed class CategoryEnsurer(ICategoryStore categories) : ICategoryEnsurer
{
    public async Task<string> EnsureAsync(string category, CancellationToken cancellationToken)
    {
        var path = CategoryPaths.Normalise(category);
        if (await categories.GetAsync(path, cancellationToken).ConfigureAwait(false) is null)
        {
            await categories.UpsertAsync(new AudienceCategory(path, null), cancellationToken).ConfigureAwait(false);
        }

        return path;
    }
}
