using Corvus.Mcp.Server.Tawk.Core.Media;
using Corvus.Mcp.Server.Tawk.ResourceAccess.Media;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

/// <summary>Holds file contents by path. A path it does not hold is missing.</summary>
public sealed class FakeMediaFiles : IMediaFiles
{
    public Dictionary<string, byte[]> Contents { get; } = new(StringComparer.Ordinal);

    public void Check(string path, long maxBytes)
    {
        if (!Contents.TryGetValue(path, out var content))
        {
            throw new MediaException("missing");
        }

        if (content.Length > maxBytes)
        {
            throw new MediaException("too big");
        }
    }

    public Task<ReadOnlyMemory<byte>> ReadAsync(string path, long maxBytes, CancellationToken cancellationToken)
    {
        Check(path, maxBytes);
        return Task.FromResult<ReadOnlyMemory<byte>>(Contents[path]);
    }
}
