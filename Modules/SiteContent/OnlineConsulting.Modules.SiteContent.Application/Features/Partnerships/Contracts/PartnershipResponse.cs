using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Contracts;

public record PartnershipResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Title,
    string CompanyName,
    string Description,
    string WebsiteUrl,
    Guid? PhotoMediaAssetId,
    int DisplayOrder,
    Dictionary<string, object>? Metadata,
    List<PartnershipSocialLinkResponse> SocialLinks)
{
    public static PartnershipResponse FromDomain(Partnership entity, List<PartnershipSocialLinkResponse> socialLinks) => new(
        entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.Title, entity.CompanyName,
        entity.Description, entity.WebsiteUrl, entity.PhotoMediaAssetId, entity.DisplayOrder,
        MetadataSerializer.Deserialize(entity.Metadata), socialLinks);
}
