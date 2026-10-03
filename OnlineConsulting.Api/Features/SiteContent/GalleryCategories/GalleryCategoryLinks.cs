using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.DeleteGalleryCategory;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.UpdateGalleryCategory;

namespace OnlineConsulting.Api.Features.SiteContent.GalleryCategories;

public sealed class GalleryCategoryLinks() : ManagedContentLinks<GalleryCategoryResponse, UpdateGalleryCategoryCommand, DeleteGalleryCategoryCommand>("UpdateGalleryCategory", "DeleteGalleryCategory", resource => resource.Id);
