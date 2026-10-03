using OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;

namespace OnlineConsulting.Maui.Infrastructure;

/// <summary>Singleton bridge from the MAUI Window's Resumed event (raised on the UI thread) to the Blazor side.</summary>
public sealed class MauiAppResumeSource : IAppResumeSource
{
    public event Action? Resumed;

    /// <summary>Called by App's Window when the app returns to the foreground.</summary>
    public void RaiseResumed() => Resumed?.Invoke();
}
