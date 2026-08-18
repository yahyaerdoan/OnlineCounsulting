using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Equipment.Application.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Abstractions;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.GetAllEquipmentItems;

public record GetAllEquipmentItemsQuery(PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<EquipmentItemResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [EquipmentOperationClaims.Admin, EquipmentOperationClaims.Read];
}

public class GetAllEquipmentItemsHandler(IEquipmentItemRepository repository) : IRequestHandler<GetAllEquipmentItemsQuery, OperationDataResult<Paginate<EquipmentItemResponse>>>
{
    public async Task<OperationDataResult<Paginate<EquipmentItemResponse>>> Handle(GetAllEquipmentItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await repository.GetListAsync(index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<EquipmentItemResponse>
        {
            Items = [.. items.Items.Select(EquipmentItemResponse.FromDomain)],
            Index = items.Index,
            Size = items.Size,
            Count = items.Count,
            Pages = items.Pages,
        };

        return Result.Success(response, "Equipment retrieved successfully.");
    }
}
