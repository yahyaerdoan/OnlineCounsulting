using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/site-content/partnerships/query's response shape.</summary>
public record PartnershipResponse(
    Guid Id, string FirstName, string LastName, string Email, string Title, string CompanyName, string Description, string WebsiteUrl,
    Guid? PhotoMediaAssetId, int DisplayOrder, List<PartnershipSocialLinkResponse> SocialLinks, string Kind = PartnershipKinds.Partner) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(FirstName), nameof(LastName), nameof(CompanyName)];

    public bool IsTeam => Kind == PartnershipKinds.Team;
}

/// <summary>Mirrors SiteContent's PartnershipKinds: which About page section a card appears in.</summary>
public static class PartnershipKinds
{
    public const string Partner = "Partner";
    public const string Team = "Team";
}
