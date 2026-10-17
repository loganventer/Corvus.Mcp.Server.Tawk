using System.Globalization;
using Corvus.Mcp.Server.Tawk.Engines;

namespace Corvus.Mcp.Server.Tawk.Managers;

internal static class ScheduleNudges
{
    public static string Describe(ScheduleNudge nudge) =>
        nudge.Offset == TimeSpan.Zero
            ? string.Empty
            : string.Create(CultureInfo.InvariantCulture, $" tawk-mcp moved it by {nudge.Offset.TotalSeconds:+0.000;-0.000} s so it does not land on the exact minute.");
}
