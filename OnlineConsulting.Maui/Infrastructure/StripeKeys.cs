namespace OnlineConsulting.Maui.Infrastructure;

/// <summary>Stripe publishable key for the native head, which has no appsettings.json; public by design, never the secret key.</summary>
public static class StripeKeys
{
    private const string EnvironmentVariableName = "STRIPE_PUBLISHABLE_KEY";

    private const string DevDefaultPublishableKey = "pk_test_51RW0m694AGEBtTE1nSx8F1IjePXWxQyxVDT4btUJg5sYkV3wA05IoMYJYNKIDrdd5lnNotHTi5jm7mYgp1KE1iba00lbBJO9p4";

    public static string PublishableKey =>
        Environment.GetEnvironmentVariable(EnvironmentVariableName) is { Length: > 0 } configured
            ? configured
            : DevDefaultPublishableKey;
}
