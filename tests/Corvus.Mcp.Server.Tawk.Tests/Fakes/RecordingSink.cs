using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.Managers;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

public sealed class RecordingSink : IEventSink
{
    public List<LiveUpdate> Updates { get; } = [];

    public Task OnUpdateAsync(LiveUpdate update, CancellationToken cancellationToken)
    {
        lock (Updates)
        {
            Updates.Add(update);
        }

        return Task.CompletedTask;
    }
}
