namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>Newsletter blurb shared by the footer and the phone Menu page.</summary>
public static class NewsletterCopy
{
    public static string Lead(bool isPlatform) => isPlatform
        ? "Product updates and tips for growing your service business."
        : "Seasonal maintenance tips and offers, straight to your inbox.";
}
