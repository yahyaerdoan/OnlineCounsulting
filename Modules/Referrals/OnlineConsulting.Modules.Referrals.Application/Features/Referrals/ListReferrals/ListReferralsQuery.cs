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
using OnlineConsulting.SharedKernel.Identity;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.ListReferrals;

/// <summary>All referrals for staff, one page at a time, with the referrer's and referred user's email and user name.</summary>
public record ListReferralsQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<AdminReferralResponse>>>, ISecureAddRequest, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(Referral.Code), nameof(Referral.Status)]);

    public string[] Roles => [ReferralsOperationClaims.Admin, ReferralsOperationClaims.Read];
}

public class ListReferralsHandler(IReferralRepository repository, IUserContactReader contactReader)
    : IRequestHandler<ListReferralsQuery, OperationDataResult<Paginate<AdminReferralResponse>>>
{
    public async Task<OperationDataResult<Paginate<AdminReferralResponse>>> Handle(ListReferralsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: r => r.CreatedDate, tieBreaker: r => r.Id, cancellationToken: cancellationToken);

        var users = (await contactReader.GetContactsAsync([.. paged.Items.SelectMany(r => new[] { r.ReferrerUserId, r.ReferredUserId }).Distinct()], cancellationToken))
            .ToDictionary(c => c.Id);

        var response = new Paginate<AdminReferralResponse>
        {
            Items = [.. paged.Items.Select(r => AdminReferralResponse.FromDomain(r, users.GetValueOrDefault(r.ReferrerUserId), users.GetValueOrDefault(r.ReferredUserId)))],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Referrals retrieved successfully.");
    }
}
