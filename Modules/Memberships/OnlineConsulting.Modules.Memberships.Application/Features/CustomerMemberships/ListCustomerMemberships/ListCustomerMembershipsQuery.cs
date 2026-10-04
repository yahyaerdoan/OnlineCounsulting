using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Contracts;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.ListCustomerMemberships;

/// <summary>Search matches the member's name or email, the plan name or the status; View narrows by CustomerMembershipListViews.</summary>
public record ListCustomerMembershipsQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null, string? Search = null, string? View = null)
    : IRequest<OperationDataResult<Paginate<CustomerMembershipResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Read];
}

public class ListCustomerMembershipsHandler(ICustomerMembershipRepository repository, IMembershipPlanRepository planRepository, IUserContactReader contactReader)
    : IRequestHandler<ListCustomerMembershipsQuery, OperationDataResult<Paginate<CustomerMembershipResponse>>>
{
    public async Task<OperationDataResult<Paginate<CustomerMembershipResponse>>> Handle(ListCustomerMembershipsQuery request, CancellationToken cancellationToken)
    {
        var query = request.View switch
        {
            CustomerMembershipListViews.Active => repository.Query().Where(m => m.Status == CustomerMembershipStatuses.Active && !m.CancelAtPeriodEnd),

            CustomerMembershipListViews.Ending => repository.Query().Where(m => m.CancelAtPeriodEnd && m.Status != CustomerMembershipStatuses.Cancelled),

            CustomerMembershipListViews.NeedsAttention => repository.Query().Where(m =>
                m.Status == CustomerMembershipStatuses.PastDue || m.Status == CustomerMembershipStatuses.PendingPayment || m.Status == CustomerMembershipStatuses.Failed),

            CustomerMembershipListViews.Paused => repository.Query().Where(m => m.Status == CustomerMembershipStatuses.Paused),

            CustomerMembershipListViews.Cancelled => repository.Query().Where(m => m.Status == CustomerMembershipStatuses.Cancelled),

            _ => repository.Query(),
        };

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();

            var userIds = await contactReader.FindUserIdsAsync(term, cancellationToken: cancellationToken);

            var planIds = (await planRepository.GetAllAsync(p => p.Name.Contains(term), withDeleted: true, cancellationToken: cancellationToken)).Select(p => p.Id).ToList();

            query = query.Where(m => userIds.Contains(m.UserId) || planIds.Contains(m.MembershipPlanId) || m.Status.Contains(term));
        }

        var paged = await query.ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: m => m.StartDate, tieBreaker: m => m.Id, cancellationToken);

        var contacts = (await contactReader.GetContactsAsync([.. paged.Items.Select(m => m.UserId).Distinct()], cancellationToken)).ToDictionary(c => c.Id);

        var pagePlanIds = paged.Items.Select(m => m.MembershipPlanId).Distinct().ToList();

        var plans = pagePlanIds.Count == 0
            ? []
            : (await planRepository.GetAllAsync(p => pagePlanIds.Contains(p.Id), withDeleted: true, cancellationToken: cancellationToken)).ToDictionary(p => p.Id);

        var response = new Paginate<CustomerMembershipResponse>
        {
            Items = [.. paged.Items.Select(m => CustomerMembershipResponse.FromDomain(m, contacts.GetValueOrDefault(m.UserId), plans.GetValueOrDefault(m.MembershipPlanId)))],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Customer memberships retrieved successfully.");
    }
}
