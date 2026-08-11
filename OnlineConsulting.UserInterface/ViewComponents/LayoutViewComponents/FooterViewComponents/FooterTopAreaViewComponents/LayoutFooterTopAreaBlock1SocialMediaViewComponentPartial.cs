using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SocialMediaDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.FooterViewComponents.FooterTopAreaViewComponents;

public class LayoutFooterTopAreaBlock1SocialMediaViewComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.SocialMediaService.GetAllAsync<ResultAllSocialMediaAccountsWithIconDto>(false, true);
        return View(result.Data ?? Enumerable.Empty<ResultAllSocialMediaAccountsWithIconDto>().AsQueryable());
    }
}
