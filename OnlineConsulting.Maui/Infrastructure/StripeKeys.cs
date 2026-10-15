namespace OnlineConsulting.Maui.Infrastructure;

/// <summary>Stripe publishable key for the native head, which has no appsettings.json; public by design, never the secret key.</summary>
public static class StripeKeys
{
    private const string EnvironmentVariableName = "STRIPE_PUBLISHABLE_KEY";

    private const string DevDefaultPublishableKey = "pk_test_51RW0m0GMoiP6iLTN22Sl4UACB7xqLJKrOPNTWyd3r5sASVoQzkIBIpMwY55Mfe5L86uPCcM23ez7IUG5zd0KylQH00qTEYf1VC";

    public static string PublishableKey =>
        Environment.GetEnvironmentVariable(EnvironmentVariableName) is { Length: > 0 } configured
            ? configured
            : DevDefaultPublishableKey;
}
