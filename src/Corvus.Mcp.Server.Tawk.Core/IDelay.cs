namespace Corvus.Mcp.Server.Tawk.Core;

public interface IDelay
{
    Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken);
}
