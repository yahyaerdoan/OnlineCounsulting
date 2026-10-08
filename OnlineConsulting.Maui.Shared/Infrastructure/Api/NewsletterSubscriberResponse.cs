using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/inquiries/newsletter/query's response shape.</summary>
public record NewsletterSubscriberResponse(Guid Id, string Email, DateTimeOffset CreatedDate) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Email)];
}
