namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/site-content/partnerships/query's response shape.</summary>
public record PartnershipResponse(
    Guid Id, string FirstName, string LastName, string Email, string Title, string CompanyName, string Description, string WebsiteUrl,
    Guid? PhotoMediaAssetId, int DisplayOrder, List<PartnershipSocialLinkResponse> SocialLinks) : IQueryableFields
{
    public static string[] SearchFields => [nameof(FirstName), nameof(LastName), nameof(CompanyName)];
}
