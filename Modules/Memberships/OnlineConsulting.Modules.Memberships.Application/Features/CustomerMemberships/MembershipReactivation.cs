using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Globalization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships;

/// <summary>Shared by the member's own reactivate and the admin's, so both undo the pending cancellation the same way.</summary>
public static class MembershipReactivation
{
    public static async Task<OperationResult> RunAsync(CustomerMembership membership, ICustomerMembershipRepository repository, ISubscriptionGateway subscriptionGateway,
        ITenantTimeZoneReader timeZoneReader, CancellationToken cancellationToken)
    {
        if (!membership.CanBeReactivated)
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

        membership.Reactivate();

        _ = await repository.UpdateAsync(membership, cancellationToken: cancellationToken);

        var zone = await timeZoneReader.GetAsync(membership.TenantId, cancellationToken);
        var renewal = membership.RenewalDate?.InZone(zone).ToString("MMMM d, yyyy", CultureInfo.GetCultureInfo("en-US")) ?? "your next billing date";

        return Result.Success($"Membership reactivated. It will renew on {renewal} as usual.");
    }
}
