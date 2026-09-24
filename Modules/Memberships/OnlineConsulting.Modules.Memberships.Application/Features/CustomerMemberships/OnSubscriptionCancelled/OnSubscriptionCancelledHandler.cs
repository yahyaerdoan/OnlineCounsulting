using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.OnSubscriptionCancelled;

/// <summary>Handles provider-initiated cancellations; no-op if already Cancelled since our own API path updates status synchronously.</summary>
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
