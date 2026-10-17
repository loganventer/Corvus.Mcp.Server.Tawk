using Corvus.Mcp.Server.Tawk.Core.Memory;

namespace Corvus.Mcp.Server.Tawk.Engines.Memory;

public interface IStyleBaselineCalculator
{
    /// <summary>Averages the style of the given messages. Only numbers are kept; the text is not.</summary>
    StyleBaseline Calculate(IReadOnlyList<string> messages);
}
