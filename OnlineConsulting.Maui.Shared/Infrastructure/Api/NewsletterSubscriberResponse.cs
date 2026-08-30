namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/inquiries/newsletter/query's response shape.</summary>
public record NewsletterSubscriberResponse(Guid Id, string Email, DateTimeOffset CreatedDate) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Email)];
}
