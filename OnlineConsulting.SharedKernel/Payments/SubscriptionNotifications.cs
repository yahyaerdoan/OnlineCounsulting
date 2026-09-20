using MediatR;

namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Published by SubscriptionWebhook after a provider confirms a lifecycle event; ReferenceId is opaque, handled by whichever module recognizes it as its own CustomerMembership id.</summary>
public record SubscriptionRenewedNotification(string ReferenceId, string ProviderSubscriptionId, DateTimeOffset CurrentPeriodEnd) : INotification;

public record SubscriptionCancelledNotification(string ReferenceId, string ProviderSubscriptionId) : INotification;

public record SubscriptionPaymentFailedNotification(string ReferenceId, string ProviderSubscriptionId) : INotification;
