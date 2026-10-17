namespace Corvus.Mcp.Server.Tawk.ResourceAccess;

/// <summary>
/// Finds tawk's control socket: an explicit path first, then CORVUS_TAWK_CONTROL_SOCKET,
/// then $XDG_RUNTIME_DIR/corvus/tawk/control.sock, then ~/.local/state/corvus/tawk/control.sock.
/// </summary>
public sealed class ControlSocketLocator : IControlSocketLocator
{
    public const string EnvironmentVariable = "CORVUS_TAWK_CONTROL_SOCKET";

    private readonly string? _explicitPath;
    private readonly Func<string, string?> _getEnvironment;
    private readonly string _homeDirectory;

    public ControlSocketLocator(string? explicitPath, Func<string, string?> getEnvironment, string homeDirectory)
    {
        ArgumentNullException.ThrowIfNull(getEnvironment);
        _explicitPath = explicitPath;
        _getEnvironment = getEnvironment;
        _homeDirectory = homeDirectory;
    }

    public string Locate()
    {
        if (!string.IsNullOrWhiteSpace(_explicitPath))
        {
            return _explicitPath;
        }

        var fromEnvironment = _getEnvironment(EnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment;
        }

        var runtime = _getEnvironment("XDG_RUNTIME_DIR");
        if (!string.IsNullOrWhiteSpace(runtime))
        {
            return Path.Combine(runtime, "corvus", "tawk", "control.sock");
        }

        return Path.Combine(_homeDirectory, ".local", "state", "corvus", "tawk", "control.sock");
    }
}
