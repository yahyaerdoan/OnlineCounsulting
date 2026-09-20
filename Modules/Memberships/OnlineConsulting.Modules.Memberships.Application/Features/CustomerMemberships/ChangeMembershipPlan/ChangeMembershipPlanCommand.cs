using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.ChangeMembershipPlan;

/// <summary>Upgrade/downgrade for an already-Active membership - swaps the provider-side subscription's
/// price in place (prorated), unlike SubscribeToMembership which creates a brand new subscription.
/// UserId is always resolved server-side, never trusted from the client.</summary>
public record ChangeMembershipPlanCommand(Guid UserId, Guid NewMembershipPlanId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class ChangeMembershipPlanHandler(ICustomerMembershipRepository membershipRepository, IMembershipPlanRepository planRepository, ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<ChangeMembershipPlanCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ChangeMembershipPlanCommand request, CancellationToken cancellationToken)
    {
        var membership = await membershipRepository.GetAsync(m => m.UserId == request.UserId && m.Status == CustomerMembershipStatuses.Active, cancellationToken: cancellationToken);

        if (membership is null)
        {
            return Result.NotFound(CustomerMembershipMessages.NoActiveMembership);
        }

        if (membership.MembershipPlanId == request.NewMembershipPlanId)
        {
            return Result.BadRequest(CustomerMembershipMessages.AlreadyOnThisPlan);
        }

        var newPlan = await planRepository.GetAsync(p => p.Id == request.NewMembershipPlanId, cancellationToken: cancellationToken);

        if (newPlan is null || newPlan.ProviderPriceId is null || !newPlan.IsActive)
        {
            return Result.NotFound(string.Format(CustomerMembershipMessages.MembershipPlanNotFoundFormat, request.NewMembershipPlanId));
        }

        if (membership.ProviderSubscriptionId is { } subscriptionId)
        {
            var failure = await PaymentGatewayCall.RunAsync(() => subscriptionGateway.UpdateSubscriptionPriceAsync(subscriptionId, newPlan.ProviderPriceId, cancellationToken), CustomerMembershipMessages.PlanChangeFailed);
            if (failure is not null)
            {
                return failure;
            }
        }

        membership.MembershipPlanId = newPlan.Id;

        _ = await membershipRepository.UpdateAsync(membership);

        return Result.Success($"Switched to the {newPlan.Name} plan successfully.");
    }
}
