using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.DeleteGalleryItem;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.UpdateGalleryItem;

namespace OnlineConsulting.Api.Features.SiteContent.GalleryItems;

public sealed class GalleryItemLinks() : ManagedContentLinks<GalleryItemResponse, UpdateGalleryItemCommand, DeleteGalleryItemCommand>("UpdateGalleryItem", "DeleteGalleryItem", resource => resource.Id);
