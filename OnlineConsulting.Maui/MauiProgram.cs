using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using OnlineConsulting.Maui.Infrastructure;
using OnlineConsulting.Maui.Infrastructure.Api;
using OnlineConsulting.Maui.Infrastructure.Auth;
using OnlineConsulting.Maui.Shared;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using OnlineConsulting.Maui.Shared.Layout;

namespace OnlineConsulting.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        _ = builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                _ = fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        _ = builder.Services.AddMauiBlazorWebView();
        _ = builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.TopEnd;
            config.SnackbarConfiguration.RequireInteraction = false;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 5000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });

        _ = builder.Services.AddMauiSharedInfrastructure(typeof(MauiProgram).Assembly);
        _ = builder.Services.AddSingleton<IPlatformInfo, MauiPlatformInfo>();

        _ = builder.Services.AddAuthorizationCore();

        _ = builder.Services.AddScoped<SecureStorageAccessTokenProvider>();
        _ = builder.Services.AddScoped<IAccessTokenProvider>(sp => sp.GetRequiredService<SecureStorageAccessTokenProvider>());
        _ = builder.Services.AddScoped<MauiAuthenticationStateProvider>();
        _ = builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<MauiAuthenticationStateProvider>());
        _ = builder.Services.AddScoped<IAuthSession, MauiAuthSession>();

        static void ConfigureApiClient(HttpClient client)
        {
            client.BaseAddress = new Uri(ApiEndpoint.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(15);
        }

        _ = builder.Services.AddHttpClient(ApiHttpClientNames.Anonymous, ConfigureApiClient)
            .ConfigurePrimaryHttpMessageHandler(CreatePrimaryHandler);

        _ = builder.Services.AddHttpClient<IApiClient, ApiClient>(ConfigureApiClient)
            .ConfigurePrimaryHttpMessageHandler(CreatePrimaryHandler);

        if (AppEnvironment.IsDevelopment)
        {
            _ = builder.Services.AddBlazorWebViewDeveloperTools();
            _ = builder.Logging.AddDebug();
        }

        return builder.Build();
    }

    // #if DEBUG on purpose, not AppEnvironment.IsDevelopment - must not exist in a Release binary.
    private static HttpMessageHandler CreatePrimaryHandler()
    {
        var handler = new HttpClientHandler();
#if DEBUG
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif
        return handler;
    }
}
