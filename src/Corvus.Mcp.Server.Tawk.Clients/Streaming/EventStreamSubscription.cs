using System.Threading.Channels;

namespace Corvus.Mcp.Server.Tawk.Clients.Streaming;

public sealed class EventStreamSubscription(IReadOnlyList<StreamEvent> replay, ChannelReader<StreamEvent> live, Action unsubscribe) : IDisposable
{
    public IReadOnlyList<StreamEvent> Replay { get; } = replay;

    public ChannelReader<StreamEvent> Live { get; } = live;

    public void Dispose() => unsubscribe();
}
