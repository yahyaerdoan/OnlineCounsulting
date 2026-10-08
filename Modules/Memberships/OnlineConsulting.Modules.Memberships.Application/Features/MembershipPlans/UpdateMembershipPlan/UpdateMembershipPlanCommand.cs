using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.UpdateMembershipPlan;

/// <summary>Only local fields - never touches ProviderProductId/ProviderPriceId. Provider prices are immutable, so a real price change requires creating a new plan (out of scope for this phase).</summary>
public record UpdateMembershipPlanCommand(Guid Id, string Name, int IncludedVisitsPerYear, decimal DiscountPercent, decimal CreditAmount, string? Benefits)
    : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Write, MembershipsOperationClaims.Update];
}

public class UpdateMembershipPlanHandler(IMembershipPlanRepository repository) : IRequestHandler<UpdateMembershipPlanCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateMembershipPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await repository.GetAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);
        if (plan is null)
        {
            return Result.NotFound(string.Format(MembershipPlanMessages.MembershipPlanNotFoundFormat, request.Id));
        }

        plan.UpdateDetails(request.Name, request.IncludedVisitsPerYear, request.DiscountPercent, request.CreditAmount, request.Benefits);

        _ = await repository.UpdateAsync(plan, cancellationToken: cancellationToken);

        return Result.Success("Membership plan updated successfully.");
    }
}
