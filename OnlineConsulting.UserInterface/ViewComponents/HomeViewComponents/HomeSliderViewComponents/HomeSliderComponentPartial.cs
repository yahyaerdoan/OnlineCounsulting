using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.Admin.Features.SliderItem;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeSliderViewComponents;

public class HomeSliderComponentPartial(ISliderItemService sliderItemService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = await sliderItemService.GetAllAsync();
        return View(items);
    }
}
