using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Domain;

/// <summary>Customer-submitted photo/video, attached pre-visit so the technician can prepare; plain MediaAssetId, no navigation (MediaAsset lives in the Media module).</summary>
public class AppointmentMediaItem : SequentialGuidTenantEntity
{
    public required Guid AppointmentId { get; set; }
    public required Guid MediaAssetId { get; set; }
    public int DisplayOrder { get; set; }
}
