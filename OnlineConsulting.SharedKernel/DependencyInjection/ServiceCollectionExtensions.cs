using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace OnlineConsulting.SharedKernel.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>Convention-registers every class in the compiled OnlineConsulting.* assemblies against its
    /// matching interface(s). Loads assemblies straight from the app's own output directory instead of
    /// <see cref="AppDomain.CurrentDomain"/> (which only reflects whatever happens to be loaded already) and
    /// without referencing a marker type from any specific module - a host project can call this without
    /// taking a dependency on any module's repository/service classes.</summary>
    public static IServiceCollection AddOnlineConsultingConventionServices(this IServiceCollection services)
    {
        var assemblies = Directory.GetFiles(AppContext.BaseDirectory, "OnlineConsulting.*.dll")
            .Select(Assembly.LoadFrom)
            .ToArray();

        services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(classes => classes.Where(type => !IsRecord(type)), publicOnly: false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    // C# records (REPR Request/Response DTOs, MediatR command/query records) auto-implement
    // IEquatable<TSelf>, so AsImplementedInterfaces() above would otherwise register them as scoped
    // services too - e.g. CreateCategoryCommand(string, string, Guid) against
    // IEquatable<CreateCategoryCommand>, which DI can't construct (no service for `string`/`Guid`). That
    // only surfaces as a crash once ValidateOnBuild runs (Development environment), so compiler-generated
    // records are excluded; this scan is meant for actual services (Managers, Repositories, ...), never DTOs.
    private static bool IsRecord(Type type) => type.GetMethods().Any(m => m.Name == "<Clone>$");
}
