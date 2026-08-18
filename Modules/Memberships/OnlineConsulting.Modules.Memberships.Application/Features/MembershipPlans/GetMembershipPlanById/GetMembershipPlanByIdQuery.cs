using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Constants;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.GetMembershipPlanById;

public record GetMembershipPlanByIdQuery(Guid Id) : IRequest<OperationDataResult<MembershipPlanResponse>>;

public class GetMembershipPlanByIdHandler(IMembershipPlanRepository repository)
    : IRequestHandler<GetMembershipPlanByIdQuery, OperationDataResult<MembershipPlanResponse>>
{
    public async Task<OperationDataResult<MembershipPlanResponse>> Handle(GetMembershipPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var plan = await repository.GetAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);

        return plan is null
            ? Result.NotFound<MembershipPlanResponse>(string.Format(MembershipPlanMessages.MembershipPlanNotFoundFormat, request.Id))
            : Result.Success(MembershipPlanResponse.FromDomain(plan), "Membership plan retrieved successfully.");
    }
}
