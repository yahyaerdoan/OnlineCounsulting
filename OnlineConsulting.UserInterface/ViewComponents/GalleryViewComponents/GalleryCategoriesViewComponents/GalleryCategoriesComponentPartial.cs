using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Gallery;

namespace OnlineConsulting.UserInterface.ViewComponents.GalleryViewComponents.GalleryCategoriesViewComponents;

public class GalleryCategoriesComponentPartial(IGalleryContentService galleryContentService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() => View(await galleryContentService.GetCategoriesAsync());
}
