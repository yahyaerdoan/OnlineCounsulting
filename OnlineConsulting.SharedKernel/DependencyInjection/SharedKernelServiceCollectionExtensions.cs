using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.GuestIdentity;

namespace OnlineConsulting.SharedKernel.DependencyInjection;

public static class SharedKernelServiceCollectionExtensions
{
    /// <summary>Registers SharedKernel's own cross-cutting services, so any host project can pick them
    /// up without a reference to a specific module.</summary>
    public static IServiceCollection AddSharedKernel(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddScoped<IGuestIdAccessor, GuestIdAccessor>();

        return services;
    }
}
