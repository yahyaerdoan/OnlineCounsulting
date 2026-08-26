using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using OnlineConsulting.Maui.Shared.Infrastructure.Forms;
using OnlineConsulting.Maui.Shared.Layout;
using System.Reflection;

namespace OnlineConsulting.Maui.Shared;

public static class ServiceCollectionExtensions
{
    /// <summary>Registrations identical across every host - module discovery, auth-session
    /// plumbing shared by both the MAUI head and the Web host. Each host still registers its own
    /// IPlatformInfo/IAccessTokenProvider/IAuthSession and HttpClient setup.</summary>
    public static IServiceCollection AddMauiSharedInfrastructure(this IServiceCollection services, Assembly hostAssembly)
    {
        _ = services.AddSingleton<IUiModule>(new HostUiModule(hostAssembly));
        _ = services.AddSingleton<IUiModule, CoreUiModule>();
        _ = services.AddSingleton<UiModuleRegistry>();

        _ = services.AddCascadingAuthenticationState();

        _ = services.AddScoped<AuthenticationExpiredNotifier>();
        _ = services.AddScoped<TokenRefresher>();

        _ = services.AddTransient(typeof(FormState<>));

        return services;
    }
}
