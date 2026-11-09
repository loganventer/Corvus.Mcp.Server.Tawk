using Corvus.Mcp.Server.Tawk.Core.Transcription;
using Corvus.Mcp.Server.Tawk.Managers.Transcription;

namespace Corvus.Mcp.Server.Tawk.Tests.Managers.Transcription;

public class StallWatchTests
{
    private static readonly IProgress<TranscriptionProgress> Nowhere = new Progress<TranscriptionProgress>();

    [Test]
    public async Task A_pass_that_reports_nothing_is_stopped_after_the_limit()
    {
        using var watch = new StallWatch(Nowhere, TimeSpan.FromMilliseconds(100), CancellationToken.None);
        await Task.Delay(400);
        Assert.That(watch.Token.IsCancellationRequested, Is.True);
    }

    [Test]
    public async Task A_pass_that_keeps_reporting_runs_past_the_limit()
    {
        using var watch = new StallWatch(Nowhere, TimeSpan.FromMilliseconds(300), CancellationToken.None);
        for (var i = 0; i < 8; i++)
        {
            await Task.Delay(100);
            watch.Report(new TranscriptionProgress(TranscriptionStage.Transcribing, i * 10));
        }

        Assert.That(watch.Token.IsCancellationRequested, Is.False);
    }

    [Test]
    public void Cancelling_the_job_stops_the_pass()
    {
        using var outer = new CancellationTokenSource();
        using var watch = new StallWatch(Nowhere, TimeSpan.FromMinutes(5), outer.Token);
        outer.Cancel();
        Assert.That(watch.Token.IsCancellationRequested, Is.True);
    }
}
