using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Notifications.Dispatch;
using OnlineConsulting.Notifications.Persistence;
using OnlineConsulting.Notifications.Sending;

namespace OnlineConsulting.Notifications;

public static class NotificationsServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;
        services.AddDbContext<NotificationsDbContext>(options => options.UseSqlServer(connectionString));

        services.Configure<EmailOptions>(configuration.GetSection("Email"));
        services.Configure<OutboxDispatcherOptions>(configuration.GetSection("OutboxDispatcher"));

        services.AddScoped<IEmailSender, MailKitEmailSender>();
        services.AddHostedService<OutboxDispatcher>();

        return services;
    }
}
