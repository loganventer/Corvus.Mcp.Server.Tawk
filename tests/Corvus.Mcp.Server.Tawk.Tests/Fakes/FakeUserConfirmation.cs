using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

public sealed class FakeUserConfirmation(ConfirmationAnswer answer) : IUserConfirmation
{
    public List<string> Asked { get; } = [];

    public Task<ConfirmationAnswer> AskAsync(string summary, CancellationToken cancellationToken)
    {
        Asked.Add(summary);
        return Task.FromResult(answer);
    }
}
