using System.ComponentModel;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Tawk.Mcp.Core;
using Tawk.Mcp.Managers;

namespace Tawk.Mcp.Clients.Tools;

[McpServerToolType]
public sealed class SummaryTools(ISummaryManager summaries, IAccountScope accounts)
{
    [McpServerTool(Name = "set_summary", Destructive = false, ReadOnly = false, Idempotent = true, OpenWorld = false)]
    [Description(
        "Hand tawk the TL;DR of a message, which tawk then shows in place of the text until the user unfolds it. Use it only to answer "
        + "a channel event of type summary_wanted, with the message_id from that event: tawk asks for the chats the user put in TL;DR mode, "
        + "and refuses a summary for any other chat. Write one plain paragraph within the max_chars the event gives, in the message's own "
        + "language, saying only what the message says, and shorter than the message: tawk shows a summary only when it is shorter, so a "
        + "few words do for a short message. The user asked for these by switching TL;DR on for the chat, so call it at once for each event, "
        + "without asking them: nothing is sent to WhatsApp, nothing needs approval, and only a note in tawk changes. Every summary_wanted event "
        + "is answered exactly once, with set_summary or with decline_summary. If you cannot or will not write the summary, call decline_summary; "
        + "never leave the event unanswered and never try the same message again.")]
    public Task<CallToolResult> SetSummaryAsync(
        [Description("The id of the message, from the summary_wanted event.")] string messageId,
        [Description("The summary: one plain paragraph, no formatting, in the language the message is written in (the one most of it is in when it mixes languages). Never a translation.")] string text,
        [Description("The name of the model writing it, when you know it.")] string? model = null,
        [Description(ToolText.Account)] string? account = null,
        CancellationToken cancellationToken = default) =>
        ToolResults.RunAsync(accounts, account, () => summaries.KeepAsync(messageId, text, model, cancellationToken));

    [McpServerTool(Name = "decline_summary", Destructive = false, ReadOnly = false, Idempotent = true, OpenWorld = false)]
    [Description(
        "Tell tawk a message needs no summary, so it does not ask for it again. Call this whenever you are not going to give a summary, for "
        + "any reason at all: the message is too short for a summary to be shorter than it (reason too_short), it says nothing worth "
        + "summarising, such as a link, a sticker or a greeting (reason nothing_to_say), or you cannot write one for any other reason, "
        + "including when set_summary was refused or you simply decide not to (reason other). A summary_wanted event is never left "
        + "unanswered, and a message is never summarised again in a loop: tawk asks again, up to five times, until it gets a summary or this call, so "
        + "answering with this is what ends the request. Like set_summary it needs no approval, sends nothing to WhatsApp, and should be "
        + "called at once without asking the user. tawk refuses it for a chat that is not in TL;DR mode, and an older tawk (before 0.21.0) "
        + "says it cannot take it.")]
    public Task<CallToolResult> DeclineSummaryAsync(
        [Description("The id of the message, from the summary_wanted event.")] string messageId,
        [Description("Why: too_short, nothing_to_say or other.")] string reason,
        [Description(ToolText.Account)] string? account = null,
        CancellationToken cancellationToken = default) =>
        ToolResults.RunAsync(accounts, account, () => summaries.DeclineAsync(messageId, reason, cancellationToken));
}
