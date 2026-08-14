using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.SharedKernel.Tenancy;

public abstract class TenantEntity<TId> : Entity<TId>, ITenantEntity
{
    public Guid TenantId { get; set; }

    protected TenantEntity()
    {
    }

    protected TenantEntity(TId id) : base(id)
    {
    }
}
