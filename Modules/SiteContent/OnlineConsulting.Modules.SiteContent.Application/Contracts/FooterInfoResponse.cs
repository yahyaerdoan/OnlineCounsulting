using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Contracts;

public record FooterInfoResponse(Guid Id, string ImageUrl, string Description, int DisplayOrder, Dictionary<string, object>? Metadata)
{
    public static FooterInfoResponse FromDomain(FooterInfo entity) => new(entity.Id, entity.ImageUrl, entity.Description, entity.DisplayOrder, MetadataSerializer.Deserialize(entity.Metadata));
}
