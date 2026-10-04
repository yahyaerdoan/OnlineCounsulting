using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Contracts;

public class WorkOrderMediaItemResponse
{
    public required Guid Id { get; init; }
    public required Guid MediaAssetId { get; init; }
    public required bool IsBeforePhoto { get; init; }
    public required int DisplayOrder { get; init; }

    public static WorkOrderMediaItemResponse FromDomain(WorkOrderMediaItem item) => new()
    {
        Id = item.Id,
        MediaAssetId = item.MediaAssetId,
        IsBeforePhoto = item.IsBeforePhoto,
        DisplayOrder = item.DisplayOrder,
    };
}
