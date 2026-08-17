using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineConsulting.SharedKernel.Tenancy;

public static class TenantEntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<TEntity> ApplyTenantAndSoftDeleteFilter<TEntity>(this EntityTypeBuilder<TEntity> builder, ITenantProvider tenantProvider)
        where TEntity : TenantEntity<Guid>
    {
        builder.HasQueryFilter(x => x.TenantId == tenantProvider.TenantId && x.DeletedDate == null);
        return builder;
    }
}
