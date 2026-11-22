using System.Diagnostics;
using Corvus.Mcp.Library.Brokkr.Contracts.Managers;
using Corvus.Mcp.Server.Tawk.Host;
using Microsoft.Extensions.DependencyInjection;

namespace Corvus.Mcp.Server.Tawk.Tests.Host;

/// <summary>The user's instructions file syncs as config with memory; the bearer token beside it never leaves the machine.</summary>
[Platform(Exclude = "Win")]
public sealed class ConfigSyncTests
{
    private string _root = null!;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "tawk-config-sync-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(Path.Combine(_root, "config"));
        Directory.CreateDirectory(Path.Combine(_root, "data"));
        Git(null, "init", "--quiet", "--bare", "--initial-branch=main", Path.Combine(_root, "remote.git"));
    }

    [TearDown]
    public void TearDown() => Directory.Delete(_root, recursive: true);

    [Test]
    public async Task Instructions_sync_under_config_and_the_token_never_does()
    {
        File.WriteAllText(Path.Combine(_root, "config", "instructions.md"), "Write plainly.");
        File.WriteAllText(Path.Combine(_root, "config", "token"), "a-bearer-token-that-must-stay-here");
        var options = new TawkMcpOptions
        {
            DataFile = Path.Combine(_root, "data", "memory.db"),
            SyncRepository = Path.Combine(_root, "remote.git"),
            SyncAllowLocalRemote = true,
            InstructionsFile = Path.Combine(_root, "config", "instructions.md"),
        };
        await using var provider = TawkMcpComposition.BuildMemory(options);

        var report = await provider.GetRequiredService<ISyncManager>().SyncAsync(CancellationToken.None);

        var tree = Git(Path.Combine(_root, "remote.git"), "ls-tree", "-r", "--name-only", "main").Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.Multiple(() =>
        {
            Assert.That(report.Outcome, Is.EqualTo(SyncOutcome.Pushed));
            Assert.That(tree, Does.Contain("config/instructions.md"));
            Assert.That(tree, Has.None.Contains("token"));
            Assert.That(Git(Path.Combine(_root, "remote.git"), "show", "main:config/instructions.md"), Is.EqualTo("Write plainly."));
        });
    }

    private static string Git(string? gitDir, params string[] arguments)
    {
        var start = new ProcessStartInfo("git") { RedirectStandardOutput = true, RedirectStandardError = true };
        foreach (var argument in (gitDir is null ? [] : new[] { "--git-dir", gitDir }).Concat(arguments))
        {
            start.ArgumentList.Add(argument);
        }

        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output;
    }
}
