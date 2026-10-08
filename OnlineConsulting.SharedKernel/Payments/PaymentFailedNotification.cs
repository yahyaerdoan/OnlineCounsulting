using MediatR;

namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Published when a provider reports a failed payment; see PaymentSucceededNotification.</summary>
public record PaymentFailedNotification(string ReferenceId, string ProviderPaymentId, string ProviderName) : INotification;
