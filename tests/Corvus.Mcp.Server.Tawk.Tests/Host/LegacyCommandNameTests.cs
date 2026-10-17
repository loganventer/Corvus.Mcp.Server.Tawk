using Corvus.Mcp.Server.Tawk.Host;

namespace Corvus.Mcp.Server.Tawk.Tests.Host;

public sealed class LegacyCommandNameTests
{
    [Test]
    public void Only_the_old_command_name_is_legacy()
    {
        Assert.Multiple(() =>
        {
            Assert.That(LegacyCommandName.IsLegacy("tawk-mcp"), Is.True);
            Assert.That(LegacyCommandName.IsLegacy("/home/me/.local/bin/tawk-mcp"), Is.True);
            Assert.That(LegacyCommandName.IsLegacy("corvus-tawk-mcp"), Is.False);
            Assert.That(LegacyCommandName.IsLegacy("/home/me/.local/bin/corvus-tawk-mcp"), Is.False);
            Assert.That(LegacyCommandName.IsLegacy(null), Is.False);
            Assert.That(LegacyCommandName.IsLegacy(string.Empty), Is.False);
        });
    }
}
