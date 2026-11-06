using Corvus.Mcp.Server.Tawk.Core;
using Corvus.Mcp.Server.Tawk.Engines;

namespace Corvus.Mcp.Server.Tawk.Tests.Engines;

public class SoundListFormatterTests
{
    private readonly SoundListFormatter _formatter = new();

    [Test]
    public void No_sounds_says_what_a_chat_can_still_have() =>
        Assert.That(_formatter.Format(new SoundList([], "")), Does.Contain("no sounds").And.Contain("silent"));

    [Test]
    public void One_sound_is_singular_and_an_app_sound_outside_the_folders_is_named_as_such() =>
        Assert.That(_formatter.Format(new SoundList([new SoundEntry("ding")], "")),
            Is.EqualTo("1 sound: ding.\nThe app's sound is a file outside the sound folders."));
}
