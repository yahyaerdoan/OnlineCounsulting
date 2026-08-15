using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Services.Domain;

/// <summary>One photo or video in a Service's gallery (CoverMediaAssetId stays a single quick-reference thumbnail, this is the extended gallery). Plain MediaAssetId, no navigation - MediaAsset lives in the Media module's own DbContext. A video vs. photo is told apart by the referenced MediaAsset.ContentType, not a field here.</summary>
public class ServiceMediaItem : TenantEntity<Guid>
{
    public required Guid ServiceId { get; set; }
    public required Guid MediaAssetId { get; set; }
    public int DisplayOrder { get; set; }
}
