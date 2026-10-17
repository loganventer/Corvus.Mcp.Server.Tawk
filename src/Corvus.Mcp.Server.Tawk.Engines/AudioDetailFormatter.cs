using System.Globalization;
using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Engines;

public sealed class AudioDetailFormatter : IAudioDetailFormatter
{
    private readonly TimeZoneInfo _timeZone;

    public AudioDetailFormatter(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeZone = timeProvider.LocalTimeZone;
    }

    public string? Length(ChatMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (message.DurationS is not { } seconds || seconds <= 0)
        {
            return null;
        }

        var length = TimeSpan.FromSeconds(Math.Ceiling(seconds));
        return length.TotalHours >= 1
            ? string.Create(CultureInfo.InvariantCulture, $"{(int)length.TotalHours}:{length.Minutes:00}:{length.Seconds:00}")
            : string.Create(CultureInfo.InvariantCulture, $"{length.Minutes}:{length.Seconds:00}");
    }

    public string? Played(ChatMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (!message.FromMe || message.PlayedAt is not { } at || at <= 0)
        {
            return null;
        }

        return "played " + Local(at).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }

    private DateTimeOffset Local(long unixSeconds) =>
        TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(unixSeconds), _timeZone);
}
