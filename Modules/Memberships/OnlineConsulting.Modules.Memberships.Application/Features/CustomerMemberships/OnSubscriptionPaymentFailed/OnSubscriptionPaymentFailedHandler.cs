using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.OnSubscriptionPaymentFailed;

public class OnSubscriptionPaymentFailedHandler(ICustomerMembershipRepository repository, IMembershipNotifier notifier) : INotificationHandler<SubscriptionPaymentFailedNotification>
{
    public async Task Handle(SubscriptionPaymentFailedNotification notification, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(notification.ReferenceId, out var membershipId))
        {
            return;
        }

        var membership = await repository.GetAsync(m => m.Id == membershipId, cancellationToken: cancellationToken);

        if (membership is null || membership.IsCancelled)
        {
            return;
        }

        membership.MarkPaymentFailed(DateTimeOffset.UtcNow);

        _ = await repository.UpdateAsync(membership);

        await notifier.PaymentFailedAsync(membership, cancellationToken);
    }
}
