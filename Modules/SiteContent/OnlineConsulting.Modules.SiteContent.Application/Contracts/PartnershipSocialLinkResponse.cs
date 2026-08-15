using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Contracts;

public record PartnershipSocialLinkResponse(Guid Id, string Name, string Url, string Icon, string? IconColor)
{
    public static PartnershipSocialLinkResponse FromDomain(PartnershipSocialLink entity) =>
        new(entity.Id, entity.Name, entity.Url, entity.Icon, entity.IconColor);
}
