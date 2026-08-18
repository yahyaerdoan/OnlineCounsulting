using Hateoas;
using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Contracts;

/// <summary>A class with required init properties instead of a positional record, since records can't inherit LinkedResponse.</summary>
public class WorkOrderMediaItemResponse : LinkedResponse
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
