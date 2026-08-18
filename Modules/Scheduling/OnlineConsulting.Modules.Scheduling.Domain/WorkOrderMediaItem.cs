using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Domain;

/// <summary>One before/after photo or video in a WorkOrder's gallery - same pattern as Services.ServiceMediaItem (plain MediaAssetId, no navigation - MediaAsset lives in the Media module's own DbContext). A video vs. photo is told apart by the referenced MediaAsset.ContentType, not a field here.</summary>
public class WorkOrderMediaItem : TenantEntity<Guid>
{
    public required Guid WorkOrderId { get; set; }
    public required Guid MediaAssetId { get; set; }
    public bool IsBeforePhoto { get; set; }
    public int DisplayOrder { get; set; }
}
