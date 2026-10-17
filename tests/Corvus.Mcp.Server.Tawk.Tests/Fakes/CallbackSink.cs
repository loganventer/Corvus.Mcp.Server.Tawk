using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.Managers;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

/// <summary>Runs a callback for every update, to look at the world at that moment.</summary>
public sealed class CallbackSink(Action onUpdate) : IEventSink
{
    public Task OnUpdateAsync(LiveUpdate update, CancellationToken cancellationToken)
    {
        onUpdate();
        return Task.CompletedTask;
    }
}
