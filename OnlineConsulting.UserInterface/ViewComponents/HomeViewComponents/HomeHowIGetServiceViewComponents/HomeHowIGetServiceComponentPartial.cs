using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.Admin.Features.HowIGetService;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeHowIGetServiceViewComponents;

public class HomeHowIGetServiceComponentPartial(IHowIGetServiceService howIGetServiceService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await howIGetServiceService.GetAllAsync();
        return View(result);
    }
}
