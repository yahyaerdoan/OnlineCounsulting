using MediatR;

namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Published by the webhook endpoint after a provider confirms a payment. ReferenceId is opaque here - whichever module recognizes it as one of its own Order/Appointment ids handles it, the rest ignore it. In-process MediatR notification, not a real message bus - this is a modular monolith, every module's handlers already live in the same process.</summary>
public record PaymentSucceededNotification(string ReferenceId, string ProviderPaymentId, string ProviderName) : INotification;

public record PaymentFailedNotification(string ReferenceId, string ProviderPaymentId, string ProviderName) : INotification;
