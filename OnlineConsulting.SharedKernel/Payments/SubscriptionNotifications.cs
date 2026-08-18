using MediatR;

namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Published by SubscriptionWebhook after a provider confirms a subscription-lifecycle event. ReferenceId is opaque here - whichever module recognizes it as one of its own CustomerMembership ids handles it. In-process MediatR notification, not a real message bus - this is a modular monolith.</summary>
public record SubscriptionRenewedNotification(string ReferenceId, string ProviderSubscriptionId, DateTimeOffset CurrentPeriodEnd) : INotification;

public record SubscriptionCancelledNotification(string ReferenceId, string ProviderSubscriptionId) : INotification;

public record SubscriptionPaymentFailedNotification(string ReferenceId, string ProviderSubscriptionId) : INotification;
