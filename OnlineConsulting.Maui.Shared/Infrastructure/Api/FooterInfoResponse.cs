using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/site-content/footer-info/query's response shape.</summary>
public record FooterInfoResponse(Guid Id, string ImageUrl, string Description, int DisplayOrder) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Description)];
}
