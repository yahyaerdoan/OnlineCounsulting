using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.Admin.Features.FooterAbout;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.FooterViewComponents.FooterTopAreaViewComponents;

public class LayoutFooterTopAreaBlock1ViewComponentPartial(IFooterAboutService footerAboutService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = await footerAboutService.GetAllAsync();
        return View(items);
    }
}
