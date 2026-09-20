using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.SetMembershipPlanActive;

/// <summary>Archive/restore, not delete - a plan with live subscribers can never be hard-deleted (CustomerMembership.MembershipPlanId FK), so retiring it from sale is the only safe operation.</summary>
public record SetMembershipPlanActiveCommand(Guid Id, bool IsActive) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Write, MembershipsOperationClaims.Update];
}

public class SetMembershipPlanActiveHandler(IMembershipPlanRepository repository) : IRequestHandler<SetMembershipPlanActiveCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SetMembershipPlanActiveCommand request, CancellationToken cancellationToken)
    {
        var plan = await repository.GetAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);

        if (plan is null)
        {
            return Result.NotFound(string.Format(MembershipPlanMessages.MembershipPlanNotFoundFormat, request.Id));
        }

        plan.IsActive = request.IsActive;

        _ = await repository.UpdateAsync(plan);

        return Result.Success(request.IsActive ? "Membership plan restored successfully." : "Membership plan archived successfully.");
    }
}
