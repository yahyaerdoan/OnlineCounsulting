using OnlineConsulting.Modules.Memberships.Domain;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;

/// <summary>
/// Tells the member about membership changes they did not make themselves, and about every charge. Staff and system changes get an email
/// and an in-app notification (also pushed when a device is registered); a start or renewal gets the in-app notification next to the
/// receipt email sent by MembershipReceiptSender. Best effort: a failed send is logged and never fails the triggering operation.
/// </summary>
public interface IMembershipNotifier
{
    Task StartedAsync(CustomerMembership membership, CancellationToken cancellationToken = default);

    Task RenewedAsync(CustomerMembership membership, CancellationToken cancellationToken = default);

    Task PaymentFailedAsync(CustomerMembership membership, CancellationToken cancellationToken = default);

    Task CancelledAfterFailuresAsync(CustomerMembership membership, CancellationToken cancellationToken = default);

    Task CancelledByStaffAsync(CustomerMembership membership, CancellationToken cancellationToken = default);

    Task ReactivatedByStaffAsync(CustomerMembership membership, CancellationToken cancellationToken = default);
}
