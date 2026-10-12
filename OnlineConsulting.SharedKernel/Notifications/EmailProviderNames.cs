namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>Keyed-DI service key for each IEmailSender implementation - same role as PushProviderNames.</summary>
public static class EmailProviderNames
{
    public const string Smtp = "Smtp";
    public const string Brevo = "Brevo";
}
