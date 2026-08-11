using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;
using OnlineConsulting.UserInterface.Areas.User.ViewModels.AddressViewModels;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardAddressViewComponents;

public class DashboardAddressListComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.UserAddressService.GetAddressesAsync();
        var userAddressList = result.Data ?? Enumerable.Empty<ResultUserAddressDto>().AsQueryable();
        return View(new AddressListPageViewModel { UserAddressList = userAddressList });
    }
}
