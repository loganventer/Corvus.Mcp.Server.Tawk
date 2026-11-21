using Corvus.Mcp.Library.Brokkr.Contracts.Datasets;
using Corvus.Mcp.Library.Brokkr.Contracts.Errors;
using Corvus.Mcp.Library.Brokkr.Contracts.Managers;
using Corvus.Mcp.Server.Tawk.Clients.Tools;
using Corvus.Mcp.Server.Tawk.Tests.Fakes;

namespace Corvus.Mcp.Server.Tawk.Tests.Clients;

public class MemorySyncToolsTests
{
    private static readonly DateTimeOffset At = DateTimeOffset.FromUnixTimeSeconds(1000);

    [Test]
    public async Task One_sync_is_run_and_what_it_did_is_said()
    {
        var sync = new FixedSyncManager(new SyncReport(SyncOutcome.PulledAndPushed, At, "abc", [new DatasetResult("memory", 3, 1, [])]));

        var result = await new MemorySyncTools(sync).SyncMemoryAsync();

        Assert.Multiple(() =>
        {
            Assert.That(sync.Calls, Is.EqualTo(1));
            Assert.That(result.IsError, Is.False);
            Assert.That(ToolOutput.Text(result), Is.EqualTo("Merged 3 change(s) from the repository and pushed the result."));
        });
    }

    [Test]
    public async Task With_no_repository_it_says_so_and_is_not_an_error()
    {
        var sync = new FixedSyncManager(new SyncReport(SyncOutcome.Disabled, At, null, [], SyncErrorCode.Disabled));

        var result = await new MemorySyncTools(sync).SyncMemoryAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.False);
            Assert.That(ToolOutput.Text(result), Does.StartWith("Memory sync is off"));
        });
    }

    [Test]
    public async Task A_sync_that_failed_is_an_error_with_its_reason()
    {
        var sync = new FixedSyncManager(new SyncReport(SyncOutcome.Failed, At, null, [], SyncErrorCode.RemoteUnreachable, "Sync could not reach the repository: Permission denied."));

        var result = await new MemorySyncTools(sync).SyncMemoryAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(ToolOutput.Text(result), Is.EqualTo("Memory sync failed: Sync could not reach the repository: Permission denied."));
        });
    }

    [Test]
    public async Task Items_changed_on_both_sides_are_mentioned()
    {
        var conflict = new SyncConflict("memory", "contact_fact", "the more trusted writer won");
        var sync = new FixedSyncManager(new SyncReport(SyncOutcome.Pushed, At, "abc", [new DatasetResult("memory", 0, 1, [conflict])]));
        Assert.That(ToolOutput.Text(await new MemorySyncTools(sync).SyncMemoryAsync()), Does.EndWith("1 item(s) were changed on both sides and kept as the sync rules decided."));
    }
}
