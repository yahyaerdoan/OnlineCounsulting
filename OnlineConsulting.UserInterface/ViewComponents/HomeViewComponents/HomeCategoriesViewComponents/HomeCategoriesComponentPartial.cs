using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Home;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeCategoriesViewComponents;

public class HomeCategoriesComponentPartial(IHomeContentService homeContentService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() => View(await homeContentService.GetCategoriesAsync());
}
