using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>A tenant's subscription container - holds one Stripe Subscription made up of per-module TenantSubscriptionItem rows, not a single plan. Not tenant-scoped itself - lives alongside Tenant/ModuleOffering in the platform-owner's own schema.</summary>
public class TenantSubscription : Entity<Guid>
{
    /// <summary>Plain id, no navigation.</summary>
    public required Guid TenantId { get; set; }

    public required string Status { get; set; }
    public required DateTime StartDate { get; set; }
    public DateTime? RenewalDate { get; set; }
    public string? ProviderSubscriptionId { get; set; }
}
