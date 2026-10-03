using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.Abstractions;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Notifications;

/// <summary>The in-app inbox: written by the push sender for every notification, read/marked by the user's own requests.</summary>
public class UserNotificationRepository(AppIdentityDbContext context) : IUserNotificationRepository, IUserNotificationInbox
{
    public async Task AddAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken cancellationToken = default)
    {
        _ = context.UserNotifications.Add(new UserNotification
        {
            UserId = userId,
            Title = title,
            Body = body,
            DataJson = data is { Count: > 0 } ? JsonSerializer.Serialize(data) : null,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        _ = await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(List<UserNotification> Items, int Count)> GetPageAsync(Guid userId, int index, int size, CancellationToken cancellationToken = default)
    {
        var query = context.UserNotifications.AsNoTracking().Where(n => n.UserId == userId);
        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .ThenByDescending(n => n.Id)
            .Skip(index * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (items, count);
    }

    public Task<int> CountUnreadAsync(Guid userId, CancellationToken cancellationToken = default) =>
        context.UserNotifications.CountAsync(n => n.UserId == userId && n.ReadAt == null, cancellationToken);

    public async Task<bool> MarkReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await context.UserNotifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, cancellationToken);
        if (notification is null)
        {
            return false;
        }

        if (notification.ReadAt is null)
        {
            notification.ReadAt = DateTimeOffset.UtcNow;
            _ = await context.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    public async Task MarkAllReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var unread = await context.UserNotifications.Where(n => n.UserId == userId && n.ReadAt == null).ToListAsync(cancellationToken);
        if (unread.Count == 0)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        foreach (var notification in unread)
        {
            notification.ReadAt = now;
        }

        _ = await context.SaveChangesAsync(cancellationToken);
    }
}
