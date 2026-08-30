namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/site-content/footer-info/query's response shape.</summary>
public record FooterInfoResponse(Guid Id, string ImageUrl, string Description, int DisplayOrder) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Description)];
}
