using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Notifications.Dispatch;
using OnlineConsulting.Notifications.Persistence;
using OnlineConsulting.Notifications.Sending;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Notifications;

public static class NotificationsServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        _ = services.AddDbContext<NotificationsDbContext>(options => options.UseSqlServer(connectionString));

        _ = services.Configure<EmailOptions>(configuration.GetSection("Email"));
        _ = services.Configure<OutboxDispatcherOptions>(configuration.GetSection("OutboxDispatcher"));
        _ = services.Configure<PushOptions>(configuration.GetSection("Push"));

        _ = services.AddScoped<IEmailSender, MailKitEmailSender>();
        _ = services.AddHostedService<OutboxDispatcher>();
        services.AddPushNotificationSender(configuration);

        return services;
    }

    /// <summary>Keyed-provider pattern, same shape as Payments.AddPaymentsInfrastructure - Mock is always registered and is the default ActiveProvider; Fcm is only registered (and only ever selected) when Push:FirebaseCredentialsPath actually points at a real file, so "ActiveProvider: Fcm" without real credentials falls back to Mock instead of crashing at startup.</summary>
    private static void AddPushNotificationSender(this IServiceCollection services, IConfiguration configuration)
    {
        var pushSection = configuration.GetSection("Push");
        var activeProvider = pushSection["ActiveProvider"];
        if (string.IsNullOrWhiteSpace(activeProvider))
        {
            activeProvider = PushProviderNames.Mock;
        }

        _ = services.AddKeyedScoped<IPushNotificationSender, MockPushNotificationSender>(PushProviderNames.Mock);

        var firebaseCredentialsPath = pushSection["FirebaseCredentialsPath"];
        if (!string.IsNullOrWhiteSpace(firebaseCredentialsPath) && File.Exists(firebaseCredentialsPath))
        {
            FirebaseApp.DefaultInstance?.Delete();
            _ = FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromFile(firebaseCredentialsPath) });
            _ = services.AddKeyedScoped<IPushNotificationSender, FcmPushNotificationSender>(PushProviderNames.Fcm);
        }
        else if (activeProvider == PushProviderNames.Fcm)
        {
            activeProvider = PushProviderNames.Mock;
        }

        _ = services.AddScoped(sp => sp.GetRequiredKeyedService<IPushNotificationSender>(activeProvider));
    }
}
