using Microsoft.Extensions.Hosting;
using Corvus.Mcp.Server.Tawk.Core.Transcription;
using Corvus.Mcp.Server.Tawk.Managers.Transcription;

namespace Corvus.Mcp.Server.Tawk.Clients;

/// <summary>Runs queued transcription jobs for the life of the process, as many at once as the user allowed.</summary>
public sealed class TranscriptionWorker(ITranscriptionRunManager transcription, TranscriptionOptions options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.WhenAll(Enumerable.Range(0, Math.Max(1, options.Concurrency)).Select(_ => LoopAsync(stoppingToken))).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Shutting down.
        }
    }

    private async Task LoopAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await transcription.RunNextAsync(stoppingToken).ConfigureAwait(false);
        }
    }
}
