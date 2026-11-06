using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Engines;

/// <summary>Writes tawk's notification sounds as text for an agent.</summary>
public interface ISoundListFormatter
{
    string Format(SoundList list);
}
