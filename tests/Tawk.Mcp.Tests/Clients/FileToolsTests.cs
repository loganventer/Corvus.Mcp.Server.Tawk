using Tawk.Mcp.Clients;
using Tawk.Mcp.Clients.Tools;
using Tawk.Mcp.Core;
using Tawk.Mcp.ResourceAccess;
using Tawk.Mcp.Tests.Fakes;

namespace Tawk.Mcp.Tests.Clients;

public class FileToolsTests
{
    private TestParts _parts = null!;
    private FileTools _tools = null!;

    [SetUp]
    public void SetUp()
    {
        _parts = new TestParts();
        _parts.Control.Hello = _parts.Control.Hello with { Features = [TawkFeatures.SendFile] };
        _tools = new FileTools(_parts.Files, new AmbientAccountScope());
    }

    [Test]
    public async Task Send_file_reports_waiting_for_approval_as_progress()
    {
        _parts.Control.Answer("send_file", """{"id":"3EB0D4","file":"report.pdf","type":"document","bytes":14}""");
        _parts.Control.AskForApproval.Add("send_file");
        var progress = new FakeProgress();

        var result = await _tools.SendFileAsync("Mom", "/tmp/report.pdf", "The report", progress: progress);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.False);
            Assert.That(ToolOutput.Text(result), Does.Contain("report.pdf").And.Contain("3EB0D4"));
            Assert.That(progress.Reports.Single().Message, Is.EqualTo(ApprovalProgress.WaitingMessage));
            Assert.That((string?)_parts.Control.Last("send_file").Args!["caption"], Is.EqualTo("The report"));
        });
    }

    [Test]
    public async Task Send_file_turns_a_refusal_into_a_tool_error()
    {
        _parts.Control.Fail("send_file", new ControlError("not_found", "No such file on this computer"));

        var result = await _tools.SendFileAsync("Mom", "/tmp/nope.pdf");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(ToolOutput.Text(result), Does.Contain("No such file"));
        });
    }
}
