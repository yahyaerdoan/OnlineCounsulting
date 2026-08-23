using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using OnlineConsulting.Maui.Infrastructure;
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
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices(config =>
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

        builder.Services.AddMauiSharedInfrastructure(typeof(MauiProgram).Assembly);
        builder.Services.AddSingleton<IPlatformInfo, MauiPlatformInfo>();

        builder.Services.AddAuthorizationCore();

        builder.Services.AddScoped<SecureStorageAccessTokenProvider>();
        builder.Services.AddScoped<IAccessTokenProvider>(sp => sp.GetRequiredService<SecureStorageAccessTokenProvider>());
        builder.Services.AddScoped<MauiAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<MauiAuthenticationStateProvider>());
        builder.Services.AddScoped<IAuthSession, MauiAuthSession>();

        void ConfigureApiClient(HttpClient client)
        {
            client.BaseAddress = new Uri(ApiEndpoint.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(15);
        }

        builder.Services.AddHttpClient(ApiHttpClientNames.Anonymous, ConfigureApiClient)
            .ConfigurePrimaryHttpMessageHandler(CreatePrimaryHandler);

        builder.Services.AddHttpClient<IApiClient, ApiClient>(ConfigureApiClient)
            .ConfigurePrimaryHttpMessageHandler(CreatePrimaryHandler);

        if (AppEnvironment.IsDevelopment)
        {
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
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
