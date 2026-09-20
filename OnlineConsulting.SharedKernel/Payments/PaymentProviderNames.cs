namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Keyed-DI service key per IPaymentGateway, persisted on Payment/Order so a refund routes back to the provider that actually processed it, not whichever is active later.</summary>
public static class PaymentProviderNames
{
    public const string Mock = "Mock";
    public const string Stripe = "Stripe";
    public const string PayPal = "PayPal";
}
