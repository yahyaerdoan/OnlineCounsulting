namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Keyed-DI service key for each IPaymentGateway implementation, and the value persisted on a Payment/Order record so a refund can be routed back to the provider that actually processed it - not whichever provider happens to be active later.</summary>
public static class PaymentProviderNames
{
    public const string Mock = "Mock";
    public const string Stripe = "Stripe";
    public const string PayPal = "PayPal";
}
