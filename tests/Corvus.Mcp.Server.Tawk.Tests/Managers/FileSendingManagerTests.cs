using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.Engines;
using Corvus.Mcp.Server.Tawk.Managers;
using Corvus.Mcp.Server.Tawk.Tests.Fakes;

namespace Corvus.Mcp.Server.Tawk.Tests.Managers;

public class FileSendingManagerTests
{
    private readonly FakeTawkControl _control = new();

    private FileSendingManager Manager(bool tawkSendsFiles = true)
    {
        _control.Hello = _control.Hello with { Features = tawkSendsFiles ? [TawkFeatures.SendFile] : [TawkFeatures.Summaries] };
        return new FileSendingManager(_control, new SentFileDescriber());
    }

    [Test]
    public async Task A_file_goes_to_tawk_with_its_chat_and_caption()
    {
        _control.Answer("send_file", """{"id":"3EB0","file":"report.pdf","type":"document","bytes":14}""");

        var said = await Manager().SendFileAsync("Mom", "/tmp/report.pdf", "The report", null, CancellationToken.None);

        var sent = _control.Last("send_file").Args!;
        Assert.Multiple(() =>
        {
            Assert.That((string?)sent["chat"], Is.EqualTo("Mom"));
            Assert.That((string?)sent["file"], Is.EqualTo("/tmp/report.pdf"));
            Assert.That((string?)sent["caption"], Is.EqualTo("The report"));
            Assert.That(said, Does.Contain("report.pdf (document, 14 B)").And.Contain("3EB0"));
        });
    }

    [Test]
    public async Task No_caption_is_sent_when_none_was_given()
    {
        _control.Answer("send_file", """{"id":"3EB0","file":"a.png","type":"image","bytes":2048}""");

        await Manager().SendFileAsync("Mom", "/tmp/a.png", "  ", null, CancellationToken.None);

        Assert.That(_control.Last("send_file").Args!.ContainsKey("caption"), Is.False);
    }

    [Test]
    public void A_tawk_that_cannot_send_files_is_sent_nothing()
    {
        var refused = Assert.ThrowsAsync<TawkControlException>(
            () => Manager(tawkSendsFiles: false).SendFileAsync("Mom", "/tmp/report.pdf", null, null, CancellationToken.None));

        Assert.Multiple(() =>
        {
            Assert.That(refused!.Message, Is.EqualTo(FileSendingManager.OlderTawk));
            Assert.That(_control.Requests, Is.Empty);
        });
    }

    [Test]
    public void A_refusal_from_tawk_is_passed_on()
    {
        _control.Fail("send_file", new ControlError("bad_request", "That file is part of tawk's own data or holds secrets, and is never sent"));

        var refused = Assert.ThrowsAsync<TawkControlException>(
            () => Manager().SendFileAsync("Mom", "/home/logan/.ssh/id_rsa", null, null, CancellationToken.None));

        Assert.That(refused!.Message, Does.Contain("never sent"));
    }

    [Test]
    public void A_path_and_a_chat_are_required()
    {
        Assert.Multiple(() =>
        {
            Assert.ThrowsAsync<ArgumentException>(() => Manager().SendFileAsync("", "/tmp/a.png", null, null, CancellationToken.None));
            Assert.ThrowsAsync<ArgumentException>(() => Manager().SendFileAsync("Mom", "", null, null, CancellationToken.None));
        });
    }
}
