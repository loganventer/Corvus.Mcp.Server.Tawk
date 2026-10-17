using Corvus.Mcp.Server.Tawk.Core.Memory;

namespace Corvus.Mcp.Server.Tawk.Engines.Memory;

public interface IVoiceChecker
{
    VoiceReport Check(string draft, VoiceRules rules);
}
