using System.Text.Json.Nodes;
using Tawk.Mcp.Core;
using Tawk.Mcp.Engines;
using Tawk.Mcp.ResourceAccess;

namespace Tawk.Mcp.Managers;

public sealed class FileSendingManager(ITawkControl control, ISentFileDescriber describer) : IFileSendingManager
{
    public const string OlderTawk = "This tawk cannot send files (it needs tawk 0.22.0 or later).";

    public async Task<string> SendFileAsync(
        string chat, string file, string? caption, Action? onApprovalWaiting, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(chat);
        ArgumentException.ThrowIfNullOrEmpty(file);
        var hello = await control.ConnectAsync(cancellationToken).ConfigureAwait(false);
        if (!hello.Has(TawkFeatures.SendFile))
        {
            throw new TawkControlException(OlderTawk);
        }

        var args = new JsonObject { ["chat"] = chat, ["file"] = file };
        if (!string.IsNullOrWhiteSpace(caption))
        {
            args["caption"] = caption;
        }

        var sent = await control.RequestAsync<SentFile>("send_file", args, onApprovalWaiting, cancellationToken).ConfigureAwait(false);
        return describer.Describe(sent);
    }
}
