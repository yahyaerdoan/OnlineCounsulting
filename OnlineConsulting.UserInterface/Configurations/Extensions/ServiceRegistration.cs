using Microsoft.AspNetCore.Identity;
using NToastNotify;
using OnlineConsulting.SharedKernel.DependencyInjection;
using System.Globalization;

namespace OnlineConsulting.UserInterface.Configurations.Extensions;

public static class ServiceRegistration
{
    public static void AddUserInterfaceServiceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSharedKernel();

        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        services.AddMvc().AddNToastNotifyToastr(new ToastrOptions
        {
            ProgressBar = true,
            PositionClass = ToastPositions.BottomLeft,
            PreventDuplicates = true,
            CloseButton = true,
            NewestOnTop = true,
            TimeOut = 3000
        });

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });
        services.ConfigureApplicationCookie(options =>
        {
            var cookieBuilder = new CookieBuilder
            {
                Name = "OnlineConsultingCookie",
                HttpOnly = true, // JavaScript erişimini engeller
                SameSite = SameSiteMode.Strict, // CSRF saldırılarına karşı önlem
                SecurePolicy = CookieSecurePolicy.Always, // Sadece HTTPS üzerinden iletilmesini sağlar
                IsEssential = true, // GDPR uyumlu
            };
            cookieBuilder.Name = "OnlineConsultingCookie";
            options.LoginPath = new PathString("/Account/Login");
            options.LogoutPath = new PathString("/Account/Logout");
            options.AccessDeniedPath = new PathString("/ErrorPage/Unauthorized");
            options.Cookie = cookieBuilder;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            options.SlidingExpiration = true;
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        });
        services.AddCors(opt => opt.AddDefaultPolicy(policy => policy
       .WithOrigins("http://localhost:4200", "https://localhost:4200", "https://localhost:7113").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

    }

    public static string ToUsd(this decimal amount) => amount.ToString("C", CultureInfo.GetCultureInfo("en-US"));
}
