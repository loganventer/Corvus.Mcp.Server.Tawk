using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Engines;

/// <summary>Says in a line or two whether someone is online, when they were last seen, or why that is not known.</summary>
public interface IPresenceFormatter
{
    string Format(ChatPresence presence);
}
