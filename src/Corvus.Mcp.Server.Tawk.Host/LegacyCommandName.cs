namespace Corvus.Mcp.Server.Tawk.Host;

/// <summary>
/// Legacy: install.sh --legacy-links makes a tawk-mcp symlink to corvus-tawk-mcp, so old registrations still start.
/// Started under that name, the program says once on stderr that the name is going away. Remove with the symlink.
/// </summary>
public static class LegacyCommandName
{
    public const string Old = "tawk-mcp";

    public const string Notice = "tawk-mcp is deprecated and will be removed: run corvus-tawk-mcp instead (see CUTOVER.md).";

    /// <summary>True when argv[0], as the shell passed it, names the legacy command.</summary>
    public static bool IsLegacy(string? argv0) =>
        !string.IsNullOrEmpty(argv0) && string.Equals(Path.GetFileName(argv0), Old, StringComparison.Ordinal);

    /// <summary>argv[0] as the process was started, which keeps a symlink's own name; null where it cannot be read.</summary>
    public static string? ReadArgv0()
    {
        try
        {
            var cmdline = File.ReadAllText("/proc/self/cmdline");
            var end = cmdline.IndexOf('\0', StringComparison.Ordinal);
            return end < 0 ? cmdline : cmdline[..end];
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            return null;
        }
    }
}
