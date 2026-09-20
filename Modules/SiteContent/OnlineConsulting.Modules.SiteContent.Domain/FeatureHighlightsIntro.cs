using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

/// <summary>Singleton-in-practice: the section-level intro paragraph + cover image shown above the FeatureHighlight card grid on Home. CRUD-shaped like the rest of SiteContent even though only one row is expected.</summary>
public class FeatureHighlightsIntro : SequentialGuidTenantEntity
{
    public required string Description { get; set; }

    /// <summary>Plain id, no navigation - MediaAsset lives in the Media module's own DbContext (Partnership.PhotoMediaAssetId precedent).</summary>
    public Guid? CoverMediaAssetId { get; set; }

    public int DisplayOrder { get; set; }

    /// <summary>Free-form JSON for template-specific extras - keeps this entity from needing a new migration every time a different UI template wants a different field.</summary>
    public string? Metadata { get; set; }
}
