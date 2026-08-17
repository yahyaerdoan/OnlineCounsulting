using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Contracts;

public record GalleryCategoryResponse(Guid Id, string Name, string? Description)
{
    public static GalleryCategoryResponse FromDomain(GalleryCategory entity) => new(entity.Id, entity.Name, entity.Description);
}
