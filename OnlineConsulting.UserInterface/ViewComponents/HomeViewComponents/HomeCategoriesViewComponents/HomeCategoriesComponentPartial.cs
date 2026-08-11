using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.CategoryDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeCategoriesViewComponents;

public class HomeCategoriesComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.CategoryService.GetAllCategoriesWithImgIconsAsync<ResultCategoryDto>(false, true);
        return View(result.Data ?? Enumerable.Empty<ResultCategoryDto>().AsQueryable());
    }
}
