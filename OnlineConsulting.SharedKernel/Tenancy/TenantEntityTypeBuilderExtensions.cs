using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineConsulting.SharedKernel.Tenancy;

public static class TenantEntityTypeBuilderExtensions
{
    // Must match Core.PersistenceLayer.Repositories.Entities.QueryFilterNames.SoftDelete - EfRepositoryBase's
    // withDeleted:true ignores only this named filter, so tenant isolation stays on even when soft-deleted rows are included.
    private const string SoftDeleteFilterKey = "SoftDelete";
    private const string TenantFilterKey = "Tenant";

    public static EntityTypeBuilder<TEntity> ApplyTenantAndSoftDeleteFilter<TEntity>(this EntityTypeBuilder<TEntity> builder, ITenantProvider tenantProvider)
        where TEntity : TenantEntity<Guid>
    {
        _ = builder.HasQueryFilter(SoftDeleteFilterKey, x => x.DeletedDate == null);
        _ = builder.HasQueryFilter(TenantFilterKey, x => x.TenantId == tenantProvider.TenantId);
        // Indexes the combined predicate above - every TenantEntity query filters on it.
        _ = builder.HasIndex(x => new { x.TenantId, x.DeletedDate });
        return builder;
    }
}
