namespace Corvus.Mcp.Server.Tawk.Clients.Sessions;

public interface IClientSessionRegistry
{
    void Add(IClientSession session);

    void Remove(IClientSession session);

    IReadOnlyList<IClientSession> Sessions { get; }
}
