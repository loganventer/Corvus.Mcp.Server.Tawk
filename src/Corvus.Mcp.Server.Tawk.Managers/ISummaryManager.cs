namespace Corvus.Mcp.Server.Tawk.Managers;

/// <summary>Hands the TL;DR of a long message to tawk, which keeps it and shows it in place of the text.</summary>
public interface ISummaryManager
{
    /// <summary>Keeps <paramref name="text"/> as the summary of the message, and says so as text for an agent.</summary>
    Task<string> KeepAsync(string messageId, string text, string? model, CancellationToken cancellationToken);

    /// <summary>
    /// Tells tawk the message needs no summary, for <paramref name="reason"/> (<c>too_short</c>, <c>nothing_to_say</c> or <c>other</c>),
    /// so it is not asked for again, and says so as text for an agent.
    /// </summary>
    Task<string> DeclineAsync(string messageId, string reason, CancellationToken cancellationToken);
}
