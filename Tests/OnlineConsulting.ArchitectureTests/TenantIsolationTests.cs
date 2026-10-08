using System.Linq.Expressions;
using Core.PersistenceLayer.MultiTenancy;
using Core.PersistenceLayer.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.ArchitectureTests;

/// <summary>Tenant isolation must not depend on anyone remembering a line: contexts holding tenant rows get it from Core, every tenant entity carries
/// a filter that reads the querying context's tenant, and the filter is only ever lifted through IgnoreTenantFilter.</summary>
public class TenantIsolationTests
{
    private static readonly HashSet<string> NotIsolatedYet = ["AppIdentityDbContext"];

    private static readonly string[] SourceRoots = ["Modules", "OnlineConsulting.Api", "OnlineConsulting.SharedKernel", "OnlineConsulting.Notifications", "OnlineConsulting.Payments", "OnlineConsulting.Storage"];

    public static TheoryData<string> Contexts() => [.. ContextTypes().Select(type => type.Name)];

    [Theory]
    [MemberData(nameof(Contexts))]
    public void A_context_mapping_tenant_entities_has_tenant_isolation(string contextName)
    {
        using var context = Create(contextName);
        var tenantEntities = TenantEntityTypes(context).Select(entityType => entityType.DisplayName()).ToList();

        Assert.True(tenantEntities.Count == 0 || context.HasTenantIsolation() || NotIsolatedYet.Contains(contextName),
            $"{contextName} maps tenant entities ({string.Join(", ", tenantEntities)}) without tenant isolation: derive it from TenantDbContext, or implement ITenantScopedDbContext and call UseTenantIsolation.");
    }

    [Theory]
    [MemberData(nameof(Contexts))]
    public void Every_tenant_entity_filters_on_the_querying_contexts_tenant(string contextName)
    {
        using var context = Create(contextName);
        if (!context.HasTenantIsolation())
        {
            return;
        }

        var violations = TenantEntityTypes(context)
            .Where(entityType => entityType.BaseType is null)
            .Where(entityType => entityType.GetDeclaredQueryFilters().FirstOrDefault(filter => filter.Key == QueryFilterNames.Tenant) is not { } filter
                || !ReadsContext(filter.Expression))
            .Select(entityType => entityType.DisplayName())
            .ToList();

        Assert.True(violations.Count == 0,
            $"{contextName}: these tenant entities have no tenant filter, or one that doesn't read the querying context (a captured value would be reused for every tenant): {string.Join(", ", violations)}");
    }

    [Fact]
    public void The_tenant_filter_is_only_lifted_through_IgnoreTenantFilter()
    {
        var violations = SourceRoots
            .SelectMany(root => Directory.EnumerateFiles(Path.Combine(Solution.RootDirectory(), root), "*.cs", SearchOption.AllDirectories))
            .Where(path => !path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Any(segment => segment is "bin" or "obj" or "Migrations"))
            .Where(path => File.ReadAllText(path).Contains("IgnoreQueryFilters(", StringComparison.Ordinal))
            .Select(path => Path.GetRelativePath(Solution.RootDirectory(), path))
            .ToList();

        Assert.True(violations.Count == 0, $"Lift the tenant filter only with IgnoreTenantFilter(), so every bypass is easy to find:\n{string.Join("\n", violations)}");
    }

    private static IEnumerable<Type> ContextTypes() =>
        Solution.Layer("Infrastructure")
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(DbContext).IsAssignableFrom(type) && type is { IsAbstract: false, IsPublic: true })
            .OrderBy(type => type.Name, StringComparer.Ordinal);

    private static DbContext Create(string contextName)
    {
        var type = ContextTypes().Single(candidate => candidate.Name == contextName);
        var builder = (DbContextOptionsBuilder)(Activator.CreateInstance(typeof(DbContextOptionsBuilder<>).MakeGenericType(type))
            ?? throw new InvalidOperationException($"Could not build options for {contextName}."));
        _ = builder.UseSqlServer("Server=.;Database=ArchitectureTests;Trusted_Connection=True");

        var constructor = type.GetConstructors().Single();
        var arguments = constructor.GetParameters().Select(parameter => parameter.ParameterType switch
        {
            var options when typeof(DbContextOptions).IsAssignableFrom(options) => builder.Options,
            var tenant when tenant == typeof(ITenantContext) || tenant == typeof(ITenantProvider) => (object)new NullTenantProvider(),
            var other => throw new InvalidOperationException($"{contextName} needs a {other.Name} the test can't supply."),
        }).ToArray();

        return (DbContext)constructor.Invoke(arguments);
    }

    private static IEnumerable<IEntityType> TenantEntityTypes(DbContext context) =>
        context.Model.GetEntityTypes().Where(entityType => typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType) && !entityType.IsOwned());

    private static bool ReadsContext(LambdaExpression? filter)
    {
        var finder = new ContextReferenceFinder();
        _ = finder.Visit(filter);
        return finder.Found;
    }

    private sealed class ContextReferenceFinder : ExpressionVisitor
    {
        public bool Found { get; private set; }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            Found |= node.Value is DbContext;
            return base.VisitConstant(node);
        }
    }
}
