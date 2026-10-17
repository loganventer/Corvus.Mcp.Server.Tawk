using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Managers;

public interface IAppManager
{
    Task<string> AppStatusAsync(CancellationToken cancellationToken);

    /// <summary>Which corvus-tawk-mcp this is, and which tawk it is talking to when one is running.</summary>
    Task<string> VersionAsync(CancellationToken cancellationToken);

    /// <summary>The accounts agents may use, one to a line, with what they may do in each.</summary>
    Task<string> ListAccountsAsync(CancellationToken cancellationToken);

    /// <summary>Tells tawk what this session is working on, for its list of connected agents.</summary>
    Task<string> DescribeSessionAsync(string description, CancellationToken cancellationToken);

    Task<string> ReconnectAsync(WriteContext context, CancellationToken cancellationToken);

    Task<string> DeclineCallAsync(WriteContext context, CancellationToken cancellationToken);
}
