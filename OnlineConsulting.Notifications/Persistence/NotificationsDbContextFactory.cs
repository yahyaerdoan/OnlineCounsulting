using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Persistence;

namespace OnlineConsulting.Notifications.Persistence;

public class NotificationsDbContextFactory : IDesignTimeDbContextFactory<NotificationsDbContext>
{
    public NotificationsDbContext CreateDbContext(string[] args) => new(DesignTimeDbContextOptionsFactory.Build<NotificationsDbContext>());
}
