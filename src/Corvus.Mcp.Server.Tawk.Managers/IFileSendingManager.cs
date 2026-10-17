namespace Corvus.Mcp.Server.Tawk.Managers;

public interface IFileSendingManager
{
    /// <summary>Asks tawk to send a file from the computer it runs on. The user always answers in tawk; the call waits for them.</summary>
    Task<string> SendFileAsync(string chat, string file, string? caption, Action? onApprovalWaiting, CancellationToken cancellationToken);
}
