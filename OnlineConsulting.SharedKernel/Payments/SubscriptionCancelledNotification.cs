using MediatR;

namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Published when a provider ends a subscription; see SubscriptionRenewedNotification.</summary>
public record SubscriptionCancelledNotification(string ReferenceId, string ProviderSubscriptionId) : INotification;
