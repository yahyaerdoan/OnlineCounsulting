namespace OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;

/// <summary>Native "app came back to the foreground" signal. Registered only by the MAUI head; the web head relies on the
/// browser's visibilitychange event, which LiveUpdatesHost listens to on both.</summary>
public interface IAppResumeSource
{
    event Action? Resumed;
}
