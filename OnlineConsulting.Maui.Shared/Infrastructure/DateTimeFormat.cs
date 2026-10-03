using System.Globalization;

namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>Formats dates/times as fixed en-US, 12-hour with AM/PM - not the runtime's culture.</summary>
public static class DateTimeFormat
{
    private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

    public static string FormatDateTime(DateTimeOffset value) => value.LocalDateTime.ToString("M/d/yyyy h:mm tt", UsCulture);

    public static string FormatDate(DateTimeOffset value) => value.LocalDateTime.ToString("MMM d, yyyy", UsCulture);

    /// <summary>"Oct 3" for this year's dates, "Oct 3, 2027" otherwise; for tight spots like stat tiles.</summary>
    public static string FormatShortDate(DateTimeOffset value) =>
        value.LocalDateTime.ToString(value.LocalDateTime.Year == DateTime.Now.Year ? "MMM d" : "MMM d, yyyy", UsCulture);

    public static string FormatTime(TimeSpan value) => DateTime.Today.Add(value).ToString("h:mm tt", UsCulture);
}
