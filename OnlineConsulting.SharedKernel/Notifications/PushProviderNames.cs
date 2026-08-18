namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>Keyed-DI service key for each IPushNotificationSender implementation - same role as Payments.PaymentProviderNames.</summary>
public static class PushProviderNames
{
    public const string Mock = "Mock";
    public const string Fcm = "Fcm";
}
