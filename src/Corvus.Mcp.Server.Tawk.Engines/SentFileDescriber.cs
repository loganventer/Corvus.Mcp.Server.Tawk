using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Engines;

public sealed class SentFileDescriber : ISentFileDescriber
{
    public string Describe(SentFile sent)
    {
        ArgumentNullException.ThrowIfNull(sent);
        var what = $"{sent.File} ({sent.Type}, {ByteSizeText.Of(sent.Bytes)})";
        var head = sent.Edited
            ? $"Sent after you edited the caption in tawk: {what}, message id {sent.Id}. The final caption was:\n{sent.Text}"
            : $"Approved in tawk and queued to send: {what}, message id {sent.Id}. It goes out like any file sent from tawk.";
        return sent.Disclaimer
            ? head + "\ntawk added the user's AI disclaimer line as the caption. Do not add one yourself."
            : head;
    }
}
