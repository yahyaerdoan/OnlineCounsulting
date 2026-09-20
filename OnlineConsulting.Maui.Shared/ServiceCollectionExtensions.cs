using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using OnlineConsulting.Maui.Shared.Infrastructure.Commerce;
using OnlineConsulting.Maui.Shared.Infrastructure.Forms;
using OnlineConsulting.Maui.Shared.Infrastructure.Navigation;
using OnlineConsulting.Maui.Shared.Layout;
using System.Reflection;

namespace OnlineConsulting.Maui.Shared;

public static class ServiceCollectionExtensions
{
    /// <summary>Registrations shared by every host - each still registers its own auth/HttpClient setup.</summary>
    public static IServiceCollection AddMauiSharedInfrastructure(this IServiceCollection services, Assembly hostAssembly)
    {
        _ = services.AddSingleton<IUiModule>(new HostUiModule(hostAssembly));
        _ = services.AddSingleton<IUiModule, CoreUiModule>();
        _ = services.AddSingleton<IUiModule, CatalogUiModule>();
        _ = services.AddSingleton<IUiModule, SiteContentUiModule>();
        _ = services.AddSingleton<IUiModule, InquiriesUiModule>();
        _ = services.AddSingleton<IUiModule, CommerceUiModule>();
        _ = services.AddSingleton<UiModuleRegistry>();

        _ = services.AddCascadingAuthenticationState();

        _ = services.AddScoped<AuthenticationExpiredNotifier>();
        _ = services.AddScoped<TokenRefresher>();
        _ = services.AddScoped<BreadcrumbState>();
        _ = services.AddScoped<CartState>();

        _ = services.AddTransient(typeof(FormState<>));

        return services;
    }
}
