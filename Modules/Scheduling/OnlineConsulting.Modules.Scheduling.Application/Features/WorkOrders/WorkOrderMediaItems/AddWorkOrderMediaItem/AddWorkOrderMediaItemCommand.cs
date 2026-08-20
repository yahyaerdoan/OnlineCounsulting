using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Rules;
using OnlineConsulting.Modules.Scheduling.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using ResultHandler.Functional;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.WorkOrderMediaItems.AddWorkOrderMediaItem;

/// <summary>Attaches an already-uploaded MediaAsset (photo or video, told apart by its ContentType) to a WorkOrder's before/after gallery - same pattern as Services.AddServiceMediaItemCommand. Upload itself goes through the Media module's own UploadMediaAsset first, not duplicated here.</summary>
public record AddWorkOrderMediaItemCommand(Guid WorkOrderId, Guid MediaAssetId, bool IsBeforePhoto, int DisplayOrder = 0)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write];
}

public class AddWorkOrderMediaItemHandler(IWorkOrderMediaItemRepository mediaItemRepository, IWorkOrderRepository workOrderRepository)
    : IRequestHandler<AddWorkOrderMediaItemCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(AddWorkOrderMediaItemCommand request, CancellationToken cancellationToken)
    {
        var workOrderExists = await workOrderRepository.AnyAsync(w => w.Id == request.WorkOrderId, cancellationToken: cancellationToken);
        if (!workOrderExists)
        {
            return WorkOrderBusinessRules.WorkOrderNotFound(request.WorkOrderId).ToErrorDataResult<Guid>();
        }

        var entity = new WorkOrderMediaItem
        {
            Id = Guid.NewGuid(),
            WorkOrderId = request.WorkOrderId,
            MediaAssetId = request.MediaAssetId,
            IsBeforePhoto = request.IsBeforePhoto,
            DisplayOrder = request.DisplayOrder,
        };

        _ = await mediaItemRepository.AddAsync(entity);

        return Result.Created(entity.Id, "Work order media item added successfully.");
    }
}
