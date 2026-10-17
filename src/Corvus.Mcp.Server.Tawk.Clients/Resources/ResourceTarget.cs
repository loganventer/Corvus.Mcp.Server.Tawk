using Corvus.Mcp.Server.Tawk.Clients.Sessions;

namespace Corvus.Mcp.Server.Tawk.Clients.Resources;

public sealed record ResourceTarget(IClientSession Session, string Uri);
