using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

/// <summary>Renamed from the legacy Breadcrumb - the original name was a misnomer: this isn't a navigation breadcrumb trail, it's the title/description/background-image banner shown at the top of interior pages (confirmed against LayoutBredcrumbComponentPartial's actual usage).</summary>
public class PageBanner : TenantEntity<Guid>
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
    public int DisplayOrder { get; set; }

    /// <summary>Free-form JSON for template-specific extras - keeps this entity from needing a new migration every time a different UI template wants a different field.</summary>
    public string? Metadata { get; set; }
}
