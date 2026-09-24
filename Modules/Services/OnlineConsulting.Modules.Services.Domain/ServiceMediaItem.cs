using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Services.Domain;

/// <summary>One photo/video in a Service's extended gallery (CoverMediaAssetId is just the thumbnail); photo vs. video is told apart by MediaAsset.ContentType, not a field here.</summary>
public class ServiceMediaItem : SequentialGuidTenantEntity
{
    public required Guid ServiceId { get; set; }
    public required Guid MediaAssetId { get; set; }
    public int DisplayOrder { get; set; }
}
