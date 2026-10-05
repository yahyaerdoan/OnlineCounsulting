using MediatR;

namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Published when a renewal charge fails; see SubscriptionRenewedNotification.</summary>
public record SubscriptionPaymentFailedNotification(string ReferenceId, string ProviderSubscriptionId) : INotification;
