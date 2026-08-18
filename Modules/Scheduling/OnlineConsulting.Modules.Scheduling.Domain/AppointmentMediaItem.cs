using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Domain;

/// <summary>A customer-submitted photo/video of the issue, attached before the technician visits so they can review it and prepare (parts, tools) ahead of time. Same shape as Scheduling.WorkOrderMediaItem - plain MediaAssetId, no navigation, MediaAsset lives in the Media module's own DbContext.</summary>
public class AppointmentMediaItem : TenantEntity<Guid>
{
    public required Guid AppointmentId { get; set; }
    public required Guid MediaAssetId { get; set; }
    public int DisplayOrder { get; set; }
}
