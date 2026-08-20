using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.OnSubscriptionCancelled;

/// <summary>Handles cancellation initiated on the provider side (e.g. from the Stripe dashboard, or after repeated payment failures) - CancelMembershipHandler already updates the status synchronously for cancellations initiated from our own API, so this is a no-op if the membership is already Cancelled.</summary>
public class OnSubscriptionCancelledHandler(ICustomerMembershipRepository repository) : INotificationHandler<SubscriptionCancelledNotification>
{
    public async Task Handle(SubscriptionCancelledNotification notification, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(notification.ReferenceId, out var membershipId))
        {
            return;
        }

        var membership = await repository.GetAsync(m => m.Id == membershipId, cancellationToken: cancellationToken);
        if (membership is null || membership.Status == CustomerMembershipStatuses.Cancelled)
        {
            return;
        }

        membership.Status = CustomerMembershipStatuses.Cancelled;
        _ = await repository.UpdateAsync(membership);
    }
}
