using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Engines;

public interface IDraftReplyPlanner
{
    string Instructions(ChatSummary chat);
}
