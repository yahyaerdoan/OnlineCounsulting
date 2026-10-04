using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Contracts;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.GetMyPreviousMembership;

/// <summary>The caller's most recent ended membership with its plan, for the "rejoin" offer; 404 when they never had one or are a member now.</summary>
public record GetMyPreviousMembershipQuery(Guid UserId) : IRequest<OperationDataResult<CustomerMembershipResponse>>;

public class GetMyPreviousMembershipHandler(ICustomerMembershipRepository repository, IMembershipPlanRepository planRepository) : IRequestHandler<GetMyPreviousMembershipQuery, OperationDataResult<CustomerMembershipResponse>>
{
    public async Task<OperationDataResult<CustomerMembershipResponse>> Handle(GetMyPreviousMembershipQuery request, CancellationToken cancellationToken)
    {
        var current = await repository.GetAsync(m => m.UserId == request.UserId && m.Status != CustomerMembershipStatuses.Cancelled, cancellationToken: cancellationToken);

        if (current is not null)
        {
            return Result.NotFound<CustomerMembershipResponse>(CustomerMembershipMessages.NoPreviousMembership);
        }

        var ended = (await repository.GetListAsync(m => m.UserId == request.UserId && m.Status == CustomerMembershipStatuses.Cancelled,
            orderBy: q => q.OrderByDescending(m => m.RenewalDate ?? m.StartDate), size: RepositoryQuerySize.SingleItem, cancellationToken: cancellationToken)).Items.FirstOrDefault();

        if (ended is null)
        {
            return Result.NotFound<CustomerMembershipResponse>(CustomerMembershipMessages.NoPreviousMembership);
        }

        var plan = await planRepository.GetAsync(p => p.Id == ended.MembershipPlanId, withDeleted: true, cancellationToken: cancellationToken);

        return Result.Success(CustomerMembershipResponse.FromDomain(ended, null, plan), "Previous membership retrieved successfully.");
    }
}
