using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>A pre-checked set of ModuleOffering keys offered as a signup shortcut. Not tenant-scoped - shared catalog data. No Stripe product of its own - selecting one just pre-checks its ModuleKeys, each of which still becomes an independent TenantSubscriptionItem.</summary>
public class Bundle : Entity<Guid>
{
    public required string Name { get; set; }

    public List<string> ModuleKeys { get; set; } = [];

    public bool IsPubliclyVisible { get; set; } = true;
}
