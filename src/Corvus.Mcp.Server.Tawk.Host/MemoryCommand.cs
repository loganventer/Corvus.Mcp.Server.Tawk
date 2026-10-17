using Microsoft.Extensions.DependencyInjection;
using Corvus.Mcp.Server.Tawk.Core.Memory;
using Corvus.Mcp.Server.Tawk.Core.Sync;
using Corvus.Mcp.Server.Tawk.Managers.Knowledge;
using Corvus.Mcp.Server.Tawk.Managers.Sync;

namespace Corvus.Mcp.Server.Tawk.Host;

/// <summary>`tawk-mcp export-okf`, `import-okf` and `sync`: work on the memory database without serving MCP.</summary>
public static class MemoryCommand
{
    public static async Task<int> RunAsync(TawkMcpOptions options, TextWriter output, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(output);
        var services = TawkMcpComposition.BuildMemory(options);
        await using (services.ConfigureAwait(false))
        {
            try
            {
                switch (options.Command)
                {
                    case HostCommand.ExportOkf:
                        await output.WriteLineAsync(await services.GetRequiredService<IOkfBundleManager>()
                            .ExportAsync(options.BundlePath!, options.IncludeSensitive, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
                        return 0;
                    case HostCommand.ImportOkf:
                        await output.WriteLineAsync(await services.GetRequiredService<IOkfBundleManager>()
                            .ImportAsync(options.BundlePath!, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
                        return 0;
                    default:
                        var report = await services.GetRequiredService<IMemorySyncManager>().SyncAsync(cancellationToken).ConfigureAwait(false);
                        await output.WriteLineAsync(report.Message).ConfigureAwait(false);
                        return report.Outcome is SyncOutcome.Failed or SyncOutcome.Disabled ? 1 : 0;
                }
            }
            catch (MemoryException ex)
            {
                await output.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return 1;
            }
        }
    }
}
