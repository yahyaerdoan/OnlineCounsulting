namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>Narrow port for the in-app notification inbox (the bell) - implemented by Identity.Infrastructure. Every push is
/// also written here, so a user sees it in the app even if the push was never delivered (no device, app uninstalled).</summary>
public interface IUserNotificationInbox
{
    /// <summary>Stores one notification in the user's in-app inbox.</summary>
    Task AddAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken cancellationToken = default);
}
