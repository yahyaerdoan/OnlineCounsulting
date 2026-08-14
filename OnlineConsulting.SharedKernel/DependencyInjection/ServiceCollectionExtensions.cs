using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace OnlineConsulting.SharedKernel.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>Convention-registers every class in the compiled OnlineConsulting.* assemblies against its
    /// matching interface (project rule: implementation class name must equal the interface name minus the
    /// leading "I", e.g. <c>CategoryRepository : ICategoryRepository</c>). Loads assemblies straight from
    /// the app's own output directory instead of <see cref="AppDomain.CurrentDomain"/> (which only reflects
    /// whatever happens to be loaded already) and without referencing a marker type from any specific
    /// module - a host project can call this without taking a dependency on any module's repository/service
    /// classes.</summary>
    /// <remarks>Deliberately does not also call Scrutor's <c>AsImplementedInterfaces()</c>: every service in
    /// this codebase already follows the matching-name convention or is registered explicitly by hand, so
    /// that catch-all buys nothing here - and it actively breaks things, because MediatR command/query
    /// records (e.g. <c>RefreshTokenCommand</c>) declare real interfaces too
    /// (<c>IRequest&lt;TResponse&gt;</c>, <c>ISecureAddRequest</c>, ...) that don't happen to follow the
    /// naming convention. <c>AsImplementedInterfaces()</c> would sweep those up as if they were injectable
    /// services, and DI can't construct a record from its primitive constructor parameters - that only
    /// surfaces as a crash once <c>ValidateOnBuild</c> runs (Development environment).</remarks>
    public static IServiceCollection AddOnlineConsultingConventionServices(this IServiceCollection services)
    {
        var assemblies = Directory.GetFiles(AppContext.BaseDirectory, "OnlineConsulting.*.dll")
            .Select(Assembly.LoadFrom)
            .ToArray();

        services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(publicOnly: false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .WithScopedLifetime());

        return services;
    }
}
