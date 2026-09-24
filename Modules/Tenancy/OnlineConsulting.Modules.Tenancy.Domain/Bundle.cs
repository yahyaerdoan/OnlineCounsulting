using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>A pre-checked set of ModuleOffering keys offered as a signup shortcut - shared catalog data, not tenant-scoped, no Stripe product of its own.</summary>
public class Bundle : SequentialGuidEntity
{
    public required string Name { get; set; }

    public List<string> ModuleKeys { get; set; } = [];

    public bool IsPubliclyVisible { get; set; } = true;
}
