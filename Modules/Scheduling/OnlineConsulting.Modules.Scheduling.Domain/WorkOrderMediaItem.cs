using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Domain;

/// <summary>One before/after photo or video in a WorkOrder's gallery; photo vs. video is told apart by MediaAsset.ContentType, not a field here.</summary>
public class WorkOrderMediaItem : SequentialGuidTenantEntity
{
    public required Guid WorkOrderId { get; set; }
    public required Guid MediaAssetId { get; set; }
    public bool IsBeforePhoto { get; set; }
    public int DisplayOrder { get; set; }
}
