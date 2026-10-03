using System.Globalization;

namespace OnlineConsulting.Modules.Scheduling.Application.Common;

/// <summary>One wording for a visit's time across emails and push notifications, independent of the server's culture.</summary>
public static class AppointmentTimeText
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("en-US");

    public static string Day(DateTimeOffset start) => start.ToString("dddd, MMM d", Culture);

    public static string When(DateTimeOffset start) => start.ToString("dddd, MMM d 'at' h:mm tt", Culture);

    public static string Range(DateTimeOffset start, DateTimeOffset end) =>
        $"{start.ToString("dddd, MMMM d, yyyy", Culture)}, {start.ToString("h:mm tt", Culture)} - {end.ToString("h:mm tt", Culture)}";
}
