using System.Globalization;

namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>Formats dates/times as "M/d/yyyy h:mm tt" / "h:mm tt" - explicit en-US pattern (always
/// 12-hour with AM/PM), not the runtime's current culture, matching CurrencyFormat's approach.</summary>
public static class DateTimeFormat
{
    private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

    public static string FormatDateTime(DateTimeOffset value) => value.LocalDateTime.ToString("M/d/yyyy h:mm tt", UsCulture);

    public static string FormatTime(TimeSpan value) => DateTime.Today.Add(value).ToString("h:mm tt", UsCulture);
}
