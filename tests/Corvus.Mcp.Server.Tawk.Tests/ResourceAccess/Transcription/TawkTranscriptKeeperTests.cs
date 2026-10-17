using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.ResourceAccess.Transcription;
using Corvus.Mcp.Server.Tawk.Tests.Fakes;

namespace Corvus.Mcp.Server.Tawk.Tests.ResourceAccess.Transcription;

public class TawkTranscriptKeeperTests
{
    private readonly FakeTawkControl _control = new();

    private TawkTranscriptKeeper Keeper(bool tawkKeepsThem = true)
    {
        _control.Hello = _control.Hello with { Features = tawkKeepsThem ? [TawkFeatures.Transcripts] : null };
        return new TawkTranscriptKeeper(_control);
    }

    [Test]
    public async Task The_text_goes_to_tawk_for_one_language_and_replaces_nothing_else()
    {
        await Keeper().KeepAsync("3EB0", "af", "Hallo daar", "sonnet", CancellationToken.None);

        var sent = _control.Last("set_transcript").Args!;
        Assert.Multiple(() =>
        {
            Assert.That((string?)sent["message_id"], Is.EqualTo("3EB0"));
            Assert.That((string?)sent["language"], Is.EqualTo("af"));
            Assert.That((string?)sent["text"], Is.EqualTo("Hallo daar"));
            Assert.That((string?)sent["model"], Is.EqualTo("sonnet"));
            Assert.That(sent.ContainsKey("replace"), Is.False, "only that language's transcript is replaced");
        });
    }

    [Test]
    public void A_tawk_that_keeps_no_transcripts_is_sent_nothing_and_the_caller_is_told()
    {
        var keeper = Keeper(tawkKeepsThem: false);

        var thrown = Assert.ThrowsAsync<TawkControlException>(() => keeper.KeepAsync("3EB0", "af", "Hallo", null, CancellationToken.None));

        Assert.Multiple(() =>
        {
            Assert.That(thrown!.Message, Is.EqualTo(TawkTranscriptKeeper.OlderTawk));
            Assert.That(_control.Requests.Any(r => r.Op == "set_transcript"), Is.False);
        });
    }
}
