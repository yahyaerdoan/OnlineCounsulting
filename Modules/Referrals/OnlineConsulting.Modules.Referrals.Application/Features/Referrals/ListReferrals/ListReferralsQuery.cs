using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Referrals.Application.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Contracts;
using OnlineConsulting.Modules.Referrals.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.ListReferrals;

public record ListReferralsQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<ReferralResponse>>>, ISecureAddRequest, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(Referral.Code), nameof(Referral.Status)]);

    public string[] Roles => [ReferralsOperationClaims.Admin, ReferralsOperationClaims.Read];
}

public class ListReferralsHandler(IReferralRepository repository)
    : IRequestHandler<ListReferralsQuery, OperationDataResult<Paginate<ReferralResponse>>>
{
    public async Task<OperationDataResult<Paginate<ReferralResponse>>> Handle(ListReferralsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: r => r.CreatedDate, tieBreaker: r => r.Id, cancellationToken: cancellationToken);

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
