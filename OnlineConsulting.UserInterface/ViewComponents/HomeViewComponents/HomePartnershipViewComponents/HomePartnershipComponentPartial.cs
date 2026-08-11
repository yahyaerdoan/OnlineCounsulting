using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.PartnershipDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomePartnershipViewComponents;

public class HomePartnershipComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.PartnershipService.GetAllPartnershipsWithSocialMediasAsync<ResultPartnershipDto>(false, true);
        return View(result.Data ?? Enumerable.Empty<ResultPartnershipDto>().AsQueryable());
    }
}
