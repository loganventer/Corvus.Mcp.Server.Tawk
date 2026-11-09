using Corvus.Mcp.Server.Tawk.Core.Transcription;

namespace Corvus.Mcp.Server.Tawk.Managers.Transcription;

/// <summary>
/// Ends a pass that stops moving: every step the pass reports gives it the full time limit again, so a long
/// note heard piece by piece runs as long as it keeps going, and one that hangs is still stopped.
/// </summary>
public sealed class StallWatch : IProgress<TranscriptionProgress>, IDisposable
{
    private readonly IProgress<TranscriptionProgress> _inner;
    private readonly TimeSpan _limit;
    private readonly CancellationTokenSource _source;

    public StallWatch(IProgress<TranscriptionProgress> inner, TimeSpan limit, CancellationToken outer)
    {
        _inner = inner;
        _limit = limit;
        _source = CancellationTokenSource.CreateLinkedTokenSource(outer);
        _source.CancelAfter(limit);
    }

    /// <summary>Cancelled when the pass has gone the limit without a step, or when the outer token is.</summary>
    public CancellationToken Token => _source.Token;

    public void Report(TranscriptionProgress value)
    {
        _inner.Report(value);
        if (!_source.IsCancellationRequested)
        {
            _source.CancelAfter(_limit);
        }
    }

    public void Dispose() => _source.Dispose();
}
