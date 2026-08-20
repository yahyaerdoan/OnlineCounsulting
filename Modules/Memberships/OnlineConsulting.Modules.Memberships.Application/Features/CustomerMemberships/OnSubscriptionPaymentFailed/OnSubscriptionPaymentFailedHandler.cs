using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.OnSubscriptionPaymentFailed;

public class OnSubscriptionPaymentFailedHandler(ICustomerMembershipRepository repository, IPushNotificationSender pushNotificationSender) : INotificationHandler<SubscriptionPaymentFailedNotification>
{
    public async Task Handle(SubscriptionPaymentFailedNotification notification, CancellationToken cancellationToken)
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

        membership.Status = CustomerMembershipStatuses.PastDue;
        _ = await repository.UpdateAsync(membership);

        await pushNotificationSender.SendToUserAsync(
            membership.UserId,
            "Membership payment failed",
            "We couldn't process your latest membership payment. Please update your payment method to avoid losing your benefits.",
            new Dictionary<string, string> { ["customerMembershipId"] = membership.Id.ToString() },
            cancellationToken);
    }
}
