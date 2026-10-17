using Corvus.Mcp.Server.Tawk.Core.Memory;

namespace Corvus.Mcp.Server.Tawk.Engines.Knowledge;

public sealed class KnowledgePrecedence : IKnowledgePrecedence
{
    public bool Replaces(FactSource incoming, DateTimeOffset incomingUpdated, FactSource current, DateTimeOffset currentUpdated) =>
        incoming != current ? incoming > current : incomingUpdated > currentUpdated;
}
