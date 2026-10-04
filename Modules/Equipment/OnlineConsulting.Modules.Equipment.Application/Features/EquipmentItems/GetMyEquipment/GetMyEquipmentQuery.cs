using MediatR;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Abstractions;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.GetMyEquipment;

public record GetMyEquipmentQuery(Guid UserId) : IRequest<OperationDataResult<List<EquipmentItemResponse>>>;

public class GetMyEquipmentHandler(IEquipmentItemRepository repository) : IRequestHandler<GetMyEquipmentQuery, OperationDataResult<List<EquipmentItemResponse>>>
{
    public async Task<OperationDataResult<List<EquipmentItemResponse>>> Handle(GetMyEquipmentQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(e => e.UserId == request.UserId, cancellationToken: cancellationToken);
        var response = entities.Select(EquipmentItemResponse.FromDomain).ToList();

        return Result.Success(response, "Equipment retrieved successfully.");
    }
}
