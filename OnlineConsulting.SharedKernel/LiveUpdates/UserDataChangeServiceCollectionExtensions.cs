using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace OnlineConsulting.SharedKernel.LiveUpdates;

public static class UserDataChangeServiceCollectionExtensions
{
    /// <summary>Registers a module's entity-to-signal rules plus the shared buffer/interceptors (idempotent across modules).
    /// The host replaces the no-op publisher with its real-time transport.</summary>
    public static IServiceCollection AddUserDataChangeRules(this IServiceCollection services, Action<UserDataChangeRuleSet> configure)
    {
        var ruleSet = new UserDataChangeRuleSet();
        configure(ruleSet);
        _ = services.AddSingleton(ruleSet);

        services.TryAddSingleton<IUserDataChangePublisher, NullUserDataChangePublisher>();
        services.TryAddScoped<UserDataChangeBuffer>();
        services.TryAddScoped<UserDataChangeSaveInterceptor>();
        services.TryAddScoped<UserDataChangeTransactionInterceptor>();
        return services;
    }

    /// <summary>Adds the save/transaction interceptors that turn committed changes into user signals.</summary>
    public static DbContextOptionsBuilder AddUserDataChangeInterceptors(this DbContextOptionsBuilder options, IServiceProvider serviceProvider) =>
        options.AddInterceptors(
            serviceProvider.GetRequiredService<UserDataChangeSaveInterceptor>(),
            serviceProvider.GetRequiredService<UserDataChangeTransactionInterceptor>());
}
