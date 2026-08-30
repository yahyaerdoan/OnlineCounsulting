using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Equipment.Application.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Abstractions;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.GetAllEquipmentItemsPaged;

public record GetAllEquipmentItemsPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<EquipmentItemResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [EquipmentOperationClaims.Admin, EquipmentOperationClaims.Read];
}

public class GetAllEquipmentItemsPagedHandler(IEquipmentItemRepository repository)
    : IRequestHandler<GetAllEquipmentItemsPagedQuery, OperationDataResult<Paginate<EquipmentItemResponse>>>
{
    public async Task<OperationDataResult<Paginate<EquipmentItemResponse>>> Handle(GetAllEquipmentItemsPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.Type, tieBreaker: x => x.Id, cancellationToken);

        var response = new Paginate<EquipmentItemResponse>
        {
            Items = [.. paged.Items.Select(EquipmentItemResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Equipment retrieved successfully.");
    }
}
