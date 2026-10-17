using System.Globalization;

namespace Corvus.Mcp.Server.Tawk.Engines;

/// <summary>A size in bytes the way a person reads it: "812 B", "34 KB", "1.1 MB", "2.4 GB".</summary>
public static class ByteSizeText
{
    private static readonly string[] Units = ["KB", "MB", "GB"];

    public static string Of(long bytes)
    {
        if (bytes < 1024)
        {
            return string.Create(CultureInfo.InvariantCulture, $"{Math.Max(bytes, 0)} B");
        }

        var value = bytes / 1024.0;
        var unit = 0;
        while (value >= 1024.0 && unit < Units.Length - 1)
        {
            value /= 1024.0;
            unit++;
        }

        var format = value >= 100.0 ? "0" : "0.0";
        return string.Create(CultureInfo.InvariantCulture, $"{value.ToString(format, CultureInfo.InvariantCulture)} {Units[unit]}");
    }
}
