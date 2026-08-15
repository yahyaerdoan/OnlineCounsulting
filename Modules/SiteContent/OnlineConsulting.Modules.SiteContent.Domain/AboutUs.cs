using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

public class AboutUs : TenantEntity<Guid>
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? CoverImage { get; set; }
    public string? VideoUrl { get; set; }
    public int DisplayOrder { get; set; }

    /// <summary>Free-form JSON for template-specific extras (CTA text/link, extra styling, etc.) that don't need their own column - keeps this entity from needing a new migration every time a different UI template wants a different field.</summary>
    public string? Metadata { get; set; }
}
