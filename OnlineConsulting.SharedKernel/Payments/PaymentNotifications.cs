using MediatR;

namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Published by the webhook endpoint after a provider confirms a payment; ReferenceId is opaque, handled by whichever module recognizes it as its own Order/Appointment id.</summary>
public record PaymentSucceededNotification(string ReferenceId, string ProviderPaymentId, string ProviderName) : INotification;

public record PaymentFailedNotification(string ReferenceId, string ProviderPaymentId, string ProviderName) : INotification;
