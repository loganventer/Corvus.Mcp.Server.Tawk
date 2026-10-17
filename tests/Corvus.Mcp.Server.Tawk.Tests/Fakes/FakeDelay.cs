using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

/// <summary>Records each wait, moves the manual clock on by it, and returns at once.</summary>
public sealed class FakeDelay(ManualTimeProvider clock) : IDelay
{
    public List<TimeSpan> Delays { get; } = [];

    public async Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken)
    {
        lock (Delays)
        {
            Delays.Add(delay);
        }

        clock.Advance(delay);
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();
    }
}
