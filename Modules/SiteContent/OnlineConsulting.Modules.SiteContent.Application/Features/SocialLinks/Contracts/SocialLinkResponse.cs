using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Contracts;

public record SocialLinkResponse(Guid Id, string Name, string Url, string Icon, string? IconColor, int DisplayOrder)
{
    public static SocialLinkResponse FromDomain(SocialLink entity) =>
        new(entity.Id, entity.Name, entity.Url, entity.Icon, entity.IconColor, entity.DisplayOrder);
}
