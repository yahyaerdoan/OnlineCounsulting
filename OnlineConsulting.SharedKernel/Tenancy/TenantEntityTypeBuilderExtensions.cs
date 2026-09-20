using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineConsulting.SharedKernel.Tenancy;

public static class TenantEntityTypeBuilderExtensions
{
    // Must match Core.PersistenceLayer.Repositories.Entities.QueryFilterNames.SoftDelete - EfRepositoryBase's
    // withDeleted:true ignores only this named filter, so tenant isolation stays on even when soft-deleted rows are included.
    private const string _softDeleteFilterKey = "SoftDelete";
    private const string _tenantFilterKey = "Tenant";

    /// <summary>Applies tenant isolation and soft-delete query filters, and indexes the combined predicate since every TenantEntity query filters on it.</summary>
    public static EntityTypeBuilder<TEntity> ApplyTenantAndSoftDeleteFilter<TEntity>(this EntityTypeBuilder<TEntity> builder, ITenantProvider tenantProvider) where TEntity : TenantEntity<Guid>
    {
        _ = builder.HasQueryFilter(_softDeleteFilterKey, x => x.DeletedDate == null);
        _ = builder.HasQueryFilter(_tenantFilterKey, x => x.TenantId == tenantProvider.TenantId);
        _ = builder.HasIndex(x => new { x.TenantId, x.DeletedDate });

        return builder;
    }
}
