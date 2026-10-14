using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/site-content/landing-blocks and its /query variant.</summary>
public record LandingBlockResponse(Guid Id, string Section, string Title, string Body, string? Icon, int DisplayOrder) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Title), nameof(Body)];
}
