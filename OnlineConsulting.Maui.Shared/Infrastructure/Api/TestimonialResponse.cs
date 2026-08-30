namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/site-content/testimonials/query's response shape.</summary>
public record TestimonialResponse(Guid Id, string FirstName, string LastName, string Title, string Description, string ImageUrl, int DisplayOrder) : IQueryableFields
{
    public static string[] SearchFields => [nameof(FirstName), nameof(LastName), nameof(Title)];
}
