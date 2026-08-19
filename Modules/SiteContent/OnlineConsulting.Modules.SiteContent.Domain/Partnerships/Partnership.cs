using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Partnerships;

public class Partnership : TenantEntity<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Title { get; set; }
    public required string CompanyName { get; set; }
    public required string Description { get; set; }
    public required string WebsiteUrl { get; set; }

    /// <summary>Plain id, no navigation - MediaAsset lives in the Media module's own DbContext (Service.CoverMediaAssetId precedent).</summary>
    public Guid? PhotoMediaAssetId { get; set; }

    public int DisplayOrder { get; set; }

    /// <summary>Free-form JSON for template-specific extras - keeps this entity from needing a new migration every time a different UI template wants a different field (Testimonial precedent).</summary>
    public string? Metadata { get; set; }
}
