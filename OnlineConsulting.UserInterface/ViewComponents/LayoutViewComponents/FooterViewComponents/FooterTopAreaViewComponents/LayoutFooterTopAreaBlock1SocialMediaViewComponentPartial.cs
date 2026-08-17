using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.Admin.Features.SocialMedia;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.FooterViewComponents.FooterTopAreaViewComponents;

public class LayoutFooterTopAreaBlock1SocialMediaViewComponentPartial(ISocialMediaService socialMediaService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var links = await socialMediaService.GetAllAsync();
        return View(links);
    }
}
