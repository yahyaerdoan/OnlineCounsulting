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
}
