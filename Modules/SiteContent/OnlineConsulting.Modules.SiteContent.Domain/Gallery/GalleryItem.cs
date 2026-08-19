using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Gallery;

public class GalleryItem : TenantEntity<Guid>
{
    public required string Description { get; set; }

    /// <summary>Plain id, no navigation - MediaAsset lives in the Media module's own DbContext (Partnership.PhotoMediaAssetId/Service.CoverMediaAssetId precedent).</summary>
    public Guid? PhotoMediaAssetId { get; set; }

    public int DisplayOrder { get; set; }

    /// <summary>Free-form JSON for template-specific extras (SiteContent convention).</summary>
    public string? Metadata { get; set; }
}
