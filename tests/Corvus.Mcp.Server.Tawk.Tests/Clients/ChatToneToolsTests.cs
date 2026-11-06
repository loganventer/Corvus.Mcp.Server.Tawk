using Corvus.Mcp.Server.Tawk.Clients.Tools;
using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.Engines;
using Corvus.Mcp.Server.Tawk.Managers;
using Corvus.Mcp.Server.Tawk.ResourceAccess;
using Corvus.Mcp.Server.Tawk.Tests.Fakes;

namespace Corvus.Mcp.Server.Tawk.Tests.Clients;

public class ChatToneToolsTests
{
    private TestParts _parts = null!;
    private ChatToneTools _tools = null!;

    [SetUp]
    public void SetUp()
    {
        _parts = new TestParts();
        _parts.Control.Hello = _parts.Control.Hello with { Features = [TawkFeatures.ChatTones] };
        _tools = new ChatToneTools(new ChatToneManager(_parts.Control, _parts.Gate, new SoundListFormatter()), new AmbientAccountScope());
    }

    [Test]
    public async Task The_tools_list_sounds_and_set_a_chats_tone()
    {
        _parts.Control.Answer("list_sounds", """{"sounds":[{"id":"ding"}],"current":"ding"}""");

        var list = await _tools.ListSoundsAsync();
        var set = await _tools.SetChatToneAsync(null, "Mom", "ding");

        Assert.Multiple(() =>
        {
            Assert.That(list.IsError, Is.False);
            Assert.That(ToolOutput.Text(list), Does.StartWith("1 sound: ding."));
            Assert.That(ToolOutput.Text(set), Is.EqualTo("The chat now plays ding."));
            Assert.That((string?)_parts.Control.Last("set_chat_tone").Args!["tone"], Is.EqualTo("ding"));
        });
    }

    [Test]
    public async Task A_chats_tone_shows_in_the_chat_list()
    {
        var silent = Samples.Chat.Replace("\"archived\":false", "\"archived\":false,\"tone\":\"none\"", StringComparison.Ordinal);
        var ding = Samples.Chat.Replace("\"archived\":false", "\"archived\":false,\"tone\":\"ding\"", StringComparison.Ordinal);
        var chats = new ChatTools(_parts.Reading, _parts.Chats, new AmbientAccountScope());

        _parts.Control.Answer("list_chats", $$"""{"chats":[{{silent}}]}""");
        var quiet = ToolOutput.Text(await chats.ListChatsAsync(null, false, 10));
        _parts.Control.Answer("list_chats", $$"""{"chats":[{{ding}}]}""");
        var named = ToolOutput.Text(await chats.ListChatsAsync(null, false, 10));

        Assert.Multiple(() =>
        {
            Assert.That(quiet, Does.Contain("pinned, silent)"));
            Assert.That(named, Does.Contain("pinned, tone ding)"));
        });
    }
}
