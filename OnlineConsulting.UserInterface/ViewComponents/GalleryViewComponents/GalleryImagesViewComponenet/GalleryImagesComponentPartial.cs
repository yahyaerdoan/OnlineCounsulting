using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.GalleryViewComponents.GalleryImagesViewComponenet;

public class GalleryImagesComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.GalleryItemService.GetAllGalleryItemsAsync<ResultGalleryItemWithCategoriesDto>(false, true);
        return View(result.Data ?? Enumerable.Empty<ResultGalleryItemWithCategoriesDto>().AsQueryable());
    }
}
