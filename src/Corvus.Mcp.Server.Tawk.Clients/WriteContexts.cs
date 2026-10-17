using ModelContextProtocol;
using ModelContextProtocol.Server;
using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Clients;

public static class WriteContexts
{
    public static WriteContext For(McpServer? server, IProgress<ProgressNotificationValue>? progress) =>
        new(ElicitationConfirmation.For(server), ApprovalProgress.Reporter(progress));
}
