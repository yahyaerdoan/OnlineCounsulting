using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

/// <summary>Renamed from the legacy FooterAbout - "About" was redundant, this is the footer's own content block, not a second About page.</summary>
public class FooterInfo : TenantEntity<Guid>
{
    public required string ImageUrl { get; set; }
    public required string Description { get; set; }
    public int DisplayOrder { get; set; }

    /// <summary>Free-form JSON for template-specific extras - keeps this entity from needing a new migration every time a different UI template wants a different field.</summary>
    public string? Metadata { get; set; }
}
