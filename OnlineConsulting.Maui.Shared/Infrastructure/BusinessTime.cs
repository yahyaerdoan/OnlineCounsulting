using System.Globalization;
using Microsoft.AspNetCore.Components.Authorization;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>
/// Dates and times as the business sees them: values are converted to the business's time zone (from the API, reloaded on sign-in and
/// sign-out) and formatted as fixed en-US, 12-hour, never in the device's or the web server's zone. One per user (circuit).
/// </summary>
public sealed class BusinessTime : IDisposable
{
    private const string DefaultTimeZoneId = "America/Chicago";
    private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

    private readonly IApiClient _apiClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private TimeZoneInfo _zone = Find(DefaultTimeZoneId) ?? TimeZoneInfo.Utc;

    public BusinessTime(IApiClient apiClient, AuthenticationStateProvider authStateProvider)
    {
        _apiClient = apiClient;
        _authStateProvider = authStateProvider;
        _authStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    /// <summary>Raised when the loaded zone differs from the one in use, so already rendered times can refresh.</summary>
    public event Action? Changed;

    public TimeZoneInfo Zone => _zone;

    /// <summary>The business's current calendar day.</summary>
    public DateOnly Today => DateOnly.FromDateTime(ToLocal(DateTimeOffset.UtcNow).DateTime);

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _apiClient.GetAsync<BusinessTimeZoneResponse>(ApiRoutes.Tenancy.TimeZone, cancellationToken);
            if (result.IsSuccessful && Find(result.ResultData?.TimeZoneId) is { } loaded && loaded.Id != _zone.Id)
            {
                _zone = loaded;
                Changed?.Invoke();
            }
        }
        catch (HttpRequestException)
        {
        }
    }

    public DateTimeOffset ToLocal(DateTimeOffset value) => TimeZoneInfo.ConvertTime(value, _zone);

    /// <summary>The value in the business's zone, with any .NET date format.</summary>
    public string Format(DateTimeOffset value, string format) => ToLocal(value).ToString(format, UsCulture);

    public string FormatDateTime(DateTimeOffset value) => Format(value, "M/d/yyyy h:mm tt");

    public string FormatDate(DateTimeOffset value) => Format(value, "MMM d, yyyy");

    /// <summary>"Oct 3" for this year's dates, "Oct 3, 2027" otherwise; for tight spots like stat tiles.</summary>
    public string FormatShortDate(DateTimeOffset value) => Format(value, ToLocal(value).Year == Today.Year ? "MMM d" : "MMM d, yyyy");

    public string FormatTime(DateTimeOffset value) => Format(value, "h:mm tt");

    /// <summary>A wall-clock time that is already in the business's zone, such as an availability rule's start.</summary>
    public string FormatTime(TimeSpan value) => DateTime.MinValue.Add(value).ToString("h:mm tt", UsCulture);

    /// <summary>"just now", "5m ago", "3h ago", "2d ago", then the date.</summary>
    public string Ago(DateTimeOffset value)
    {
        var elapsed = DateTimeOffset.UtcNow - value;
        return elapsed switch
        {
            { TotalMinutes: < 1 } => "just now",
            { TotalHours: < 1 } => $"{(int)elapsed.TotalMinutes}m ago",
            { TotalDays: < 1 } => $"{(int)elapsed.TotalHours}h ago",
            { TotalDays: < 7 } => $"{(int)elapsed.TotalDays}d ago",
            _ => Format(value, "MMM d"),
        };
    }

    public void Dispose() => _authStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;

    private void OnAuthenticationStateChanged(Task<AuthenticationState> state) => _ = LoadAsync();

    private static TimeZoneInfo? Find(string? timeZoneId) =>
        timeZoneId is { Length: > 0 } id && TimeZoneInfo.TryFindSystemTimeZoneById(id, out var zone) ? zone : null;
}
