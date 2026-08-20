using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.OnSubscriptionRenewed;

/// <summary>ReferenceId is opaque to the webhook - only handled here because SubscribeToMembershipHandler set CreateSubscriptionRequest.ReferenceId to a CustomerMembership id, so it round-trips to a lookup by Id, not ProviderSubscriptionId.</summary>
public class OnSubscriptionRenewedHandler(ICustomerMembershipRepository repository) : INotificationHandler<SubscriptionRenewedNotification>
{
    public async Task Handle(SubscriptionRenewedNotification notification, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(notification.ReferenceId, out var membershipId))
        {
            return;
        }

        var membership = await repository.GetAsync(m => m.Id == membershipId, cancellationToken: cancellationToken);
        if (membership is null)
        {
            return;
        }

        membership.RenewalDate = notification.CurrentPeriodEnd;
        membership.Status = CustomerMembershipStatuses.Active;
        _ = await repository.UpdateAsync(membership);
    }
}
