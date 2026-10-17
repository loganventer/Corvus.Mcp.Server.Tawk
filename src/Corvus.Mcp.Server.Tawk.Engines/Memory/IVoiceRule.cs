using Corvus.Mcp.Server.Tawk.Core.Memory;

namespace Corvus.Mcp.Server.Tawk.Engines.Memory;

/// <summary>One style check. Returns no findings when its rule is not set, and never throws for a valid draft.</summary>
public interface IVoiceRule
{
    IEnumerable<VoiceFinding> Check(StyleFeatures draft, VoiceRules rules);
}
