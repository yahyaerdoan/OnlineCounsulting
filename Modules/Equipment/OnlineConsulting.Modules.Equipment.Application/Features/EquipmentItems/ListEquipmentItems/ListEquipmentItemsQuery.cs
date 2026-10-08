using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Equipment.Application.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Abstractions;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Contracts;
using OnlineConsulting.Modules.Equipment.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.ListEquipmentItems;

public record ListEquipmentItemsQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<EquipmentItemResponse>>>, ISecureAddRequest, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(EquipmentItem.Type), nameof(EquipmentItem.Brand), nameof(EquipmentItem.Model), nameof(EquipmentItem.SerialNumber), nameof(EquipmentItem.UserId)]);

    public string[] Roles => [EquipmentOperationClaims.Admin, EquipmentOperationClaims.Read];
}

public class ListEquipmentItemsHandler(IEquipmentItemRepository repository)
    : IRequestHandler<ListEquipmentItemsQuery, OperationDataResult<Paginate<EquipmentItemResponse>>>
{
    public async Task<OperationDataResult<Paginate<EquipmentItemResponse>>> Handle(ListEquipmentItemsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.Type, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

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
