using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SocialMediaDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.HeaderViewComponents.HeaderTopAreaViewComponents;

public class LayoutHaderTopSocialComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.SocialMediaService.
            GetAllSocialMediaAccontsWithIconAsync<ResultAllSocialMediaAccountsWithIconDto>(false);
        return View(result.Data ?? Enumerable.Empty<ResultAllSocialMediaAccountsWithIconDto>().AsQueryable());
    }
}
