using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships;

/// <summary>Shared by the member's own reactivate and the admin's, so both undo the pending cancellation the same way.</summary>
public static class MembershipReactivation
{
    public static async Task<OperationResult> RunAsync(CustomerMembership membership, ICustomerMembershipRepository repository, ISubscriptionGateway subscriptionGateway, CancellationToken cancellationToken)
    {
        if (!membership.CancelAtPeriodEnd || membership.Status == CustomerMembershipStatuses.Cancelled)
        {
            return Result.Conflict(CustomerMembershipMessages.NotReactivatable);
        }

        if (membership.ProviderSubscriptionId is { } subscriptionId)
        {
            var failure = await PaymentGatewayCall.RunAsync(() => subscriptionGateway.ReactivateSubscriptionAsync(subscriptionId, cancellationToken), CustomerMembershipMessages.ReactivateFailed);

            if (failure is not null)
            {
                return failure;
            }
        }

        membership.CancelAtPeriodEnd = false;

        _ = await repository.UpdateAsync(membership);

        var renewal = membership.RenewalDate?.ToString("MMMM d, yyyy") ?? "your next billing date";

        return Result.Success($"Membership reactivated. It will renew on {renewal} as usual.");
    }
}
