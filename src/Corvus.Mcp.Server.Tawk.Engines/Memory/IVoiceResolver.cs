using Corvus.Mcp.Server.Tawk.Core.Memory;

namespace Corvus.Mcp.Server.Tawk.Engines.Memory;

public interface IVoiceResolver
{
    /// <summary>Finds the variant for the first category that has one, walking up each path (family/spouse, then family).</summary>
    ResolvedVoice Resolve(Voice voice, IReadOnlyList<VoiceVariant> variants, IReadOnlyList<string> categories);
}
