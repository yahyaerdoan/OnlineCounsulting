using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryCategoryDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.GalleryViewComponents.GalleryCategoriesViewComponents;

public class GalleryCategoriesComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.GalleryCategoryService.GetAllAsync<ResultGalleryCategoryDto>(false, true);
        return View(result.Data ?? Enumerable.Empty<ResultGalleryCategoryDto>().AsQueryable());
    }
}
