using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>A paying customer organization on the platform. Not tenant-scoped itself - a tenant cannot belong to a tenant.</summary>
public class Tenant : Entity<Guid>
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Status { get; set; }
    public required string PrimaryContactEmail { get; set; }

    /// <summary>Stripe customer id for this tenant's billing.</summary>
    public string? ProviderCustomerId { get; set; }

    /// <summary>Id of the Identity User who is this tenant's actual owner - the first admin created at
    /// signup (CreateTenantAdminCommand), set via SetTenantOwnerCommand right after. Distinct from any other
    /// Admin-role user later invited into the same tenant - only the owner gets owner-only protections
    /// (see TenantOwnerProtection in the Identity module). Nullable because tenants created before this field
    /// existed have no owner recorded.</summary>
    public Guid? OwnerUserId { get; set; }
}
