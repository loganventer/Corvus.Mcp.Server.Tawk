using Corvus.Mcp.Server.Tawk.ResourceAccess;

namespace Corvus.Mcp.Server.Tawk.Tests.Fakes;

public sealed class FakeAdminTokenSource(string? token) : IAdminTokenSource
{
    public string? Read() => token;
}
