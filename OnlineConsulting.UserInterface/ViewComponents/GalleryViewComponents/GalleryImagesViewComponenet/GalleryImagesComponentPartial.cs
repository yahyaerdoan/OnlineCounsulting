using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Gallery;

namespace OnlineConsulting.UserInterface.ViewComponents.GalleryViewComponents.GalleryImagesViewComponenet;

public class GalleryImagesComponentPartial(IGalleryContentService galleryContentService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() => View(await galleryContentService.GetItemsAsync());
}
