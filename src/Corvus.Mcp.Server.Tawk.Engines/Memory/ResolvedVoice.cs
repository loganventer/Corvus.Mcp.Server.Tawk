using Corvus.Mcp.Server.Tawk.Core.Memory;

namespace Corvus.Mcp.Server.Tawk.Engines.Memory;

/// <summary>The voice to write in for one audience: the base guide, the variant that matched (if any), and the rules in force.</summary>
public sealed record ResolvedVoice(Voice Voice, VoiceVariant? Variant, VoiceRules Rules);
