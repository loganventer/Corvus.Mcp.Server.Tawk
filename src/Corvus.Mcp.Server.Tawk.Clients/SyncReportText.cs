using System.Globalization;
using Corvus.Mcp.Library.Brokkr.Contracts.Errors;
using Corvus.Mcp.Library.Brokkr.Contracts.Managers;

namespace Corvus.Mcp.Server.Tawk.Clients;

/// <summary>What a memory sync did, in the words the sync tool and the sync command show.</summary>
public static class SyncReportText
{
    public static string Describe(SyncReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        var merged = report.Datasets.Sum(d => d.LocalChanged);
        var conflicts = report.Datasets.Sum(d => d.Conflicts.Count);
        var text = report.Outcome switch
        {
            SyncOutcome.Disabled => "Memory sync is off: it needs a repository (CORVUS_TAWK_MCP_SYNC_REPO), reached over SSH with a key that may write to it.",
            SyncOutcome.Busy => "Another corvus-tawk-mcp on this computer is syncing memory right now; this one left the cycle out.",
            SyncOutcome.UpToDate => "Memory is already in step with the repository.",
            SyncOutcome.Pulled => string.Create(CultureInfo.InvariantCulture, $"Merged {merged} change(s) from the repository; nothing needed pushing."),
            SyncOutcome.Pushed => "Pushed this computer's memory to the repository.",
            SyncOutcome.PulledAndPushed => string.Create(CultureInfo.InvariantCulture, $"Merged {merged} change(s) from the repository and pushed the result."),
            _ => "Memory sync failed: " + (report.Detail ?? Reason(report.Error)),
        };
        return conflicts == 0 ? text : string.Create(CultureInfo.InvariantCulture, $"{text} {conflicts} item(s) were changed on both sides and kept as the sync rules decided.");
    }

    private static string Reason(SyncErrorCode? code) => code switch
    {
        SyncErrorCode.RemoteRefused => "the repository kept moving while this computer merged; the next sync tries again.",
        SyncErrorCode.RemoteNewerSchema => "the repository holds memory from a newer corvus-tawk-mcp; update this computer's to sync again.",
        SyncErrorCode.RemoteUnreachable => "the repository could not be reached.",
        _ => "error " + code,
    };
}
