using System.Reflection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using OnlineConsulting.Maui.Shared.Infrastructure.Forms;
using OnlineConsulting.Maui.Shared.Layout;

namespace OnlineConsulting.Maui.Shared;

public static class ServiceCollectionExtensions
{
    /// <summary>Registrations identical across every host - module discovery, auth-session
    /// plumbing shared by both the MAUI head and the Web host. Each host still registers its own
    /// IPlatformInfo/IAccessTokenProvider/IAuthSession and HttpClient setup.</summary>
    public static IServiceCollection AddMauiSharedInfrastructure(this IServiceCollection services, Assembly hostAssembly)
    {
        services.AddSingleton<IUiModule>(new HostUiModule(hostAssembly));
        services.AddSingleton<IUiModule, CoreUiModule>();
        services.AddSingleton<UiModuleRegistry>();

        services.AddCascadingAuthenticationState();

        services.AddScoped<AuthenticationExpiredNotifier>();
        services.AddScoped<TokenRefresher>();

        services.AddTransient(typeof(FormState<>));

        return services;
    }
}
