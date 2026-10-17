using System.Text.Json.Nodes;
using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.ResourceAccess;

namespace Corvus.Mcp.Server.Tawk.Managers;

public sealed class SummaryManager(ITawkControl control) : ISummaryManager
{
    public const string Kept = "tawk kept the summary.";

    public const string OlderTawk = "This tawk keeps no TL;DR summaries (it needs tawk 0.14.0 or later).";

    public const string Declined = "tawk will not ask for a summary of that message again.";

    public const string OlderTawkForDecline = "This tawk cannot be told that a message needs no summary (it needs tawk 0.21.0 or later); it will ask again.";

    public async Task<string> KeepAsync(string messageId, string text, string? model, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(messageId);
        ArgumentException.ThrowIfNullOrEmpty(text);
        var hello = await control.ConnectAsync(cancellationToken).ConfigureAwait(false);
        if (!hello.Has(TawkFeatures.Summaries))
        {
            throw new TawkControlException(OlderTawk);
        }

        var args = new JsonObject { ["message_id"] = messageId, ["text"] = text, ["model"] = model ?? string.Empty };
        await control.RequestAsync("set_summary", args, cancellationToken).ConfigureAwait(false);
        return Kept;
    }

    public async Task<string> DeclineAsync(string messageId, string reason, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(messageId);
        ArgumentException.ThrowIfNullOrEmpty(reason);
        var hello = await control.ConnectAsync(cancellationToken).ConfigureAwait(false);
        if (!hello.Has(TawkFeatures.SummaryDecline))
        {
            throw new TawkControlException(OlderTawkForDecline);
        }

        var args = new JsonObject { ["message_id"] = messageId, ["reason"] = reason };
        await control.RequestAsync("decline_summary", args, cancellationToken).ConfigureAwait(false);
        return Declined;
    }
}
