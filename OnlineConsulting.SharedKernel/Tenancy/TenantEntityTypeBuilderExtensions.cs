using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace OnlineConsulting.SharedKernel.Tenancy;

public static class TenantEntityTypeBuilderExtensions
{
    // Must match Core.PersistenceLayer.Repositories.Entities.QueryFilterNames.SoftDelete - EfRepositoryBase's
    // withDeleted:true ignores only this named filter, so tenant isolation stays on even when soft-deleted rows are included.
    private const string _softDeleteFilterKey = "SoftDelete";
    /// <summary>Name of the tenant filter, for a cross-module reader that must read a given tenant's rows with IgnoreQueryFilters.</summary>
    public const string TenantFilterKey = "Tenant";

    /// <summary>Applies tenant isolation and soft-delete query filters, and indexes the combined predicate since every TenantEntity query filters on it.
    /// The tenant filter references the context as a constant, which EF swaps for the context running each query.</summary>
    public static EntityTypeBuilder<TEntity> ApplyTenantAndSoftDeleteFilter<TEntity>(this EntityTypeBuilder<TEntity> builder, ITenantScopedDbContext context) where TEntity : TenantEntity<Guid>
    {
        var entity = Expression.Parameter(typeof(TEntity), "x");
        var currentTenantId = Expression.Property(Expression.Constant(context, context.GetType()), nameof(ITenantScopedDbContext.CurrentTenantId));
        var tenantFilter = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.Property(entity, nameof(TenantEntity<Guid>.TenantId)), currentTenantId), entity);

        _ = builder.HasQueryFilter(_softDeleteFilterKey, x => x.DeletedDate == null);
        _ = builder.HasQueryFilter(TenantFilterKey, tenantFilter);
        _ = builder.HasIndex(x => new { x.TenantId, x.DeletedDate });

        return builder;
    }
}
