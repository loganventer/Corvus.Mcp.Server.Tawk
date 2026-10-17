namespace Corvus.Mcp.Server.Tawk.Core;

public sealed record ChatInfo(
    ChatSummary Chat,
    string? About,
    IReadOnlyList<GroupMember>? Members,
    IReadOnlyList<string>? Labels = null,
    ReminderDue? Reminder = null);
