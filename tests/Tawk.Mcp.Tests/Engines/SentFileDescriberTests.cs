using Tawk.Mcp.Core;
using Tawk.Mcp.Engines;

namespace Tawk.Mcp.Tests.Engines;

public class SentFileDescriberTests
{
    private readonly SentFileDescriber _describer = new();

    [TestCase(0L, "0 B")]
    [TestCase(812L, "812 B")]
    [TestCase(34L * 1024, "34.0 KB")]
    [TestCase(1_153_434L, "1.1 MB")]
    [TestCase(3L * 1024 * 1024 * 1024, "3.0 GB")]
    [TestCase(250L * 1024 * 1024, "250 MB")]
    public void Sizes_read_the_way_a_person_reads_them(long bytes, string expected) =>
        Assert.That(ByteSizeText.Of(bytes), Is.EqualTo(expected));

    [Test]
    public void An_approved_file_is_named_with_its_kind_and_size()
    {
        var text = _describer.Describe(new SentFile("3EB0", "report.pdf", "document", 1_153_434));

        Assert.That(text, Is.EqualTo("Approved in tawk and queued to send: report.pdf (document, 1.1 MB), message id 3EB0. It goes out like any file sent from tawk."));
    }

    [Test]
    public void An_edited_caption_is_shown_as_it_went_out()
    {
        var text = _describer.Describe(new SentFile("3EB0", "report.pdf", "document", 14, Edited: true, Text: "The Q3 report"));

        Assert.That(text, Does.StartWith("Sent after you edited the caption in tawk").And.EndWith("The Q3 report"));
    }

    [Test]
    public void The_disclaimer_is_reported_so_the_model_does_not_add_one()
    {
        var text = _describer.Describe(new SentFile("3EB0", "a.png", "image", 2048, Disclaimer: true));

        Assert.That(text, Does.Contain("AI disclaimer").And.Contain("Do not add one yourself"));
    }
}
