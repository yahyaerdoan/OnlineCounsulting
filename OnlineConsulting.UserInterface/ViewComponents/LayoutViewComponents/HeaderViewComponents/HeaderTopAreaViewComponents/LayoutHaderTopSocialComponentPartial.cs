using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.Admin.Features.SocialMedia;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.HeaderViewComponents.HeaderTopAreaViewComponents;

public class LayoutHaderTopSocialComponentPartial(ISocialMediaService socialMediaService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var links = await socialMediaService.GetAllAsync();
        return View(links);
    }
}
