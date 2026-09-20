using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.GetAllMembershipPlans;

/// <summary>Public - no ISecureAddRequest - so the pricing page can list plans without authentication. IncludeArchived
/// is only meant for the admin plan list (Admin/Growth/Memberships.razor) - the public pricing page always omits it,
/// which defaults to active-only.</summary>
public record GetAllMembershipPlansQuery(PageRequest PageRequest, bool IncludeArchived = false) : IRequest<OperationDataResult<Paginate<MembershipPlanResponse>>>;

public class GetAllMembershipPlansHandler(IMembershipPlanRepository repository) : IRequestHandler<GetAllMembershipPlansQuery, OperationDataResult<Paginate<MembershipPlanResponse>>>
{
    public async Task<OperationDataResult<Paginate<MembershipPlanResponse>>> Handle(GetAllMembershipPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await repository.GetListAsync(p => request.IncludeArchived || p.IsActive, index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<MembershipPlanResponse>
        {
            Items = [.. plans.Items.Select(MembershipPlanResponse.FromDomain)],
            Index = plans.Index,
            Size = plans.Size,
            Count = plans.Count,
            Pages = plans.Pages,
        };

        return Result.Success(response, "Membership plans retrieved successfully.");
    }
}
