using System.Globalization;
using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Engines;

public sealed class SoundListFormatter : ISoundListFormatter
{
    public string Format(SoundList list)
    {
        ArgumentNullException.ThrowIfNull(list);
        if (list.Sounds.Count == 0)
        {
            return "tawk's sound folders hold no sounds. A chat can still be silent, or play the app's sound.";
        }

        var count = string.Create(CultureInfo.InvariantCulture, $"{list.Sounds.Count} sound{(list.Sounds.Count == 1 ? string.Empty : "s")}");
        var app = list.Current.Length == 0 ? "The app's sound is a file outside the sound folders." : $"The app plays {list.Current}.";
        return $"{count}: {string.Join(", ", list.Sounds.Select(s => s.Id))}.\n{app}";
    }
}
