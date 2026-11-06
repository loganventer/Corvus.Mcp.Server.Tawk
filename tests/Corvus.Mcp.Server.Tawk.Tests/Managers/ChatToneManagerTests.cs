using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.Engines;
using Corvus.Mcp.Server.Tawk.Managers;
using Corvus.Mcp.Server.Tawk.Tests.Fakes;

namespace Corvus.Mcp.Server.Tawk.Tests.Managers;

public class ChatToneManagerTests
{
    private readonly TestParts _parts = new();

    private ChatToneManager Manager(bool newTawk = true)
    {
        _parts.Control.Hello = _parts.Control.Hello with { Features = newTawk ? [TawkFeatures.ChatTones] : [TawkFeatures.Labels] };
        return new ChatToneManager(_parts.Control, _parts.Gate, new SoundListFormatter());
    }

    private static WriteContext Context => new(null, null);

    [Test]
    public async Task The_sounds_are_listed_by_name_with_the_apps_own()
    {
        _parts.Control.Answer("list_sounds", """{"sounds":[{"id":"chime"},{"id":"ding"}],"current":"ding"}""");

        var text = await Manager().ListSoundsAsync(CancellationToken.None);

        Assert.That(text, Is.EqualTo("2 sounds: chime, ding.\nThe app plays ding."));
    }

    [Test]
    public async Task A_sound_is_asked_for_by_its_name()
    {
        var text = await Manager().SetChatToneAsync("Mom", " ding ", Context, CancellationToken.None);
        var args = _parts.Control.Last("set_chat_tone").Args!;

        Assert.Multiple(() =>
        {
            Assert.That(text, Is.EqualTo("The chat now plays ding."));
            Assert.That((string?)args["chat"], Is.EqualTo("Mom"));
            Assert.That((string?)args["tone"], Is.EqualTo("ding"));
        });
    }

    [Test]
    public async Task Silence_and_the_apps_sound_say_so()
    {
        var manager = Manager();

        Assert.Multiple(async () =>
        {
            Assert.That(await manager.SetChatToneAsync("Mom", ChatTones.Silent, Context, CancellationToken.None), Is.EqualTo("The chat is silent now."));
            Assert.That(await manager.SetChatToneAsync("Mom", ChatTones.AppSound, Context, CancellationToken.None), Is.EqualTo("The chat now plays the app's sound."));
        });
    }

    [Test]
    public void An_older_tawk_is_not_asked()
    {
        var manager = Manager(newTawk: false);

        Assert.Multiple(() =>
        {
            Assert.ThrowsAsync<TawkControlException>(() => manager.SetChatToneAsync("Mom", "ding", Context, CancellationToken.None));
            Assert.ThrowsAsync<TawkControlException>(() => manager.ListSoundsAsync(CancellationToken.None));
            Assert.That(_parts.Control.Requests.Any(r => r.Op is "set_chat_tone" or "list_sounds"), Is.False);
        });
    }
}
