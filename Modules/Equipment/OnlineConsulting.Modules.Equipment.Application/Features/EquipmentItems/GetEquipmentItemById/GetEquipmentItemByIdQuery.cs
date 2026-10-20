using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Equipment.Application.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Abstractions;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.GetEquipmentItemById;

/// <summary>One unit for staff: the edit form and the work order's equipment label.</summary>
public record GetEquipmentItemByIdQuery(Guid Id) : IRequest<OperationDataResult<EquipmentItemResponse>>, ISecureAddRequest
{
    public string[] Roles => [EquipmentOperationClaims.Admin, EquipmentOperationClaims.Read];
}

public class GetEquipmentItemByIdHandler(IEquipmentItemRepository repository) : IRequestHandler<GetEquipmentItemByIdQuery, OperationDataResult<EquipmentItemResponse>>
{
    public async Task<OperationDataResult<EquipmentItemResponse>> Handle(GetEquipmentItemByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, enableTracking: false, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return Result.NotFound<EquipmentItemResponse>(string.Format(EquipmentMessages.EquipmentItemNotFoundFormat, request.Id));
        }

        return Result.Success(EquipmentItemResponse.FromDomain(entity), "Equipment item retrieved successfully.");
    }
}
