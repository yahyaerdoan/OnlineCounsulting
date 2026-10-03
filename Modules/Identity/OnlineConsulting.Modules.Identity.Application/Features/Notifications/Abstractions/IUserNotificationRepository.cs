using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Modules.Identity.Application.Features.Notifications.Abstractions;

/// <summary>Reads and read-state changes of a user's own inbox; every method is scoped to the given user id.</summary>
public interface IUserNotificationRepository
{
    Task<(List<UserNotification> Items, int Count)> GetPageAsync(Guid userId, int index, int size, CancellationToken cancellationToken = default);

    Task<int> CountUnreadAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <returns>False when the notification doesn't exist or belongs to someone else.</returns>
    Task<bool> MarkReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default);

    Task MarkAllReadAsync(Guid userId, CancellationToken cancellationToken = default);
}
