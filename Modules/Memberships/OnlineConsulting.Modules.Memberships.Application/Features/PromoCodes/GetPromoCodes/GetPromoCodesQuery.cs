using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.GetPromoCodes;

public record GetPromoCodesQuery(PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<PromoCodeResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Read];
}

public class GetPromoCodesHandler(IPromoCodeRepository repository) : IRequestHandler<GetPromoCodesQuery, OperationDataResult<Paginate<PromoCodeResponse>>>
{
    public async Task<OperationDataResult<Paginate<PromoCodeResponse>>> Handle(GetPromoCodesQuery request, CancellationToken cancellationToken)
    {
        var promoCodes = await repository.GetListAsync(orderBy: q => q.OrderBy(p => p.Id), index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<PromoCodeResponse>
        {
            Items = [.. promoCodes.Items.Select(PromoCodeResponse.FromDomain)],
            Index = promoCodes.Index,
            Size = promoCodes.Size,
            Count = promoCodes.Count,
            Pages = promoCodes.Pages,
        };

        return Result.Success(response, "Promo codes retrieved successfully.");
    }
}
