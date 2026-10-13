using Tawk.Mcp.Core;
using Tawk.Mcp.Engines;
using Tawk.Mcp.Tests.Fakes;

namespace Tawk.Mcp.Tests.Engines;

public class AudioDetailFormatterTests
{
    // 2026-09-30 18:02 UTC.
    private const long Ts = 1790791320;

    private readonly AudioDetailFormatter _formatter = new(new ManualTimeProvider());

    private static ChatMessage Note(bool fromMe = true, long? playedAt = null, double? durationS = null) =>
        new("ID1", Samples.MomJid, Samples.MomJid, "Mom", fromMe, Ts, "audio", null, "read", false, false, false, null, null, null, false, playedAt, durationS);

    [Test]
    public void Writes_a_short_length_as_minutes_and_seconds()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_formatter.Length(Note(durationS: 5)), Is.EqualTo("0:05"));
            Assert.That(_formatter.Length(Note(durationS: 125)), Is.EqualTo("2:05"));
        });
    }

    [Test]
    public void Writes_a_long_length_with_hours()
    {
        Assert.That(_formatter.Length(Note(durationS: 3909)), Is.EqualTo("1:05:09"));
    }

    [Test]
    public void Rounds_a_part_second_up_so_a_note_is_never_zero_long()
    {
        Assert.That(_formatter.Length(Note(durationS: 0.2)), Is.EqualTo("0:01"));
    }

    [Test]
    public void Has_no_length_when_it_is_not_known_or_not_positive()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_formatter.Length(Note()), Is.Null);
            Assert.That(_formatter.Length(Note(durationS: 0)), Is.Null);
            Assert.That(_formatter.Length(Note(durationS: -3)), Is.Null);
        });
    }

    [Test]
    public void Shows_the_date_and_exact_time_even_when_it_was_played_the_day_it_was_sent()
    {
        Assert.That(_formatter.Played(Note(playedAt: Ts + 360)), Is.EqualTo("played 2026-09-30 18:08:00"));
    }

    [Test]
    public void Shows_the_date_and_exact_time_when_it_was_played_days_later()
    {
        // The next day, 07:16:40 UTC.
        Assert.That(_formatter.Played(Note(playedAt: 1790839000)), Is.EqualTo("played 2026-10-01 07:16:40"));
    }

    [Test]
    public void Has_no_played_time_when_it_is_not_known_or_the_message_is_not_the_user_s()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_formatter.Played(Note()), Is.Null);
            Assert.That(_formatter.Played(Note(playedAt: 0)), Is.Null);
            Assert.That(_formatter.Played(Note(fromMe: false, playedAt: Ts + 60)), Is.Null);
        });
    }
}
