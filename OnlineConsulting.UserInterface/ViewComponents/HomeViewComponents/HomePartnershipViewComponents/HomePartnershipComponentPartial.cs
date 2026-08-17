using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Partnership;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomePartnershipViewComponents;

public class HomePartnershipComponentPartial(IPartnershipService partnershipService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = await partnershipService.GetAllWithSocialLinksAsync();
        return View(items);
    }
}
