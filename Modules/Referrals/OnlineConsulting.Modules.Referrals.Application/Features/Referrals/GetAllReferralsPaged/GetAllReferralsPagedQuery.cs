using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Referrals.Application.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.GetAllReferralsPaged;

public record GetAllReferralsPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<ReferralResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [ReferralsOperationClaims.Admin, ReferralsOperationClaims.Read];
}

public class GetAllReferralsPagedHandler(IReferralRepository repository)
    : IRequestHandler<GetAllReferralsPagedQuery, OperationDataResult<Paginate<ReferralResponse>>>
{
    public async Task<OperationDataResult<Paginate<ReferralResponse>>> Handle(GetAllReferralsPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: r => r.CreatedDate, tieBreaker: r => r.Id, cancellationToken);

        var response = new Paginate<ReferralResponse>
        {
            Items = [.. paged.Items.Select(ReferralResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Referrals retrieved successfully.");
    }
}
