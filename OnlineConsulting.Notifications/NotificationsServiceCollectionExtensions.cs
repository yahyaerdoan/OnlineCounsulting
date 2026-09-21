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
    /// <summary>Registers the notifications DbContext, options, email sender, outbox dispatcher, and push notification sender.</summary>
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

    /// <summary>Fcm is only registered when Push:FirebaseCredentialsPath points at a real file; otherwise "ActiveProvider: Fcm" falls back to Mock instead of crashing at startup.</summary>
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

            var credential = CredentialFactory.FromFileAsync(firebaseCredentialsPath, null, CancellationToken.None).GetAwaiter().GetResult();

            _ = FirebaseApp.Create(new AppOptions { Credential = credential });
            _ = services.AddKeyedScoped<IPushNotificationSender, FcmPushNotificationSender>(PushProviderNames.Fcm);
        }
        else if (activeProvider == PushProviderNames.Fcm)
        {
            activeProvider = PushProviderNames.Mock;
        }

        _ = services.AddScoped(sp => sp.GetRequiredKeyedService<IPushNotificationSender>(activeProvider));
    }
}
