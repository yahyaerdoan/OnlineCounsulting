using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.CheckoutViewComponents.CheckoutBillingDetailsViewComponents;

public class CheckoutBillingDetailsComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(Guid cartId)
    {
        ViewBag.cartId = cartId;

        var resultShipping = await serviceManager.UserAddressService.GetShippingAddressAsync();
        var resultBilling = await serviceManager.UserAddressService.GetBillingAddressAsync();
        var result = await serviceManager.UserAddressService.GetAddressesAsync(false);

        var model = new CheckoutAddressViewModel
        {
            ShippingAddress = resultShipping.IsSuccessful ? resultShipping.Data : null,
            BillingAddress = resultBilling.IsSuccessful ? resultBilling.Data : null,
            AllAddresses = result.IsSuccessful && result.Data is not null ? result.Data : Enumerable.Empty<ResultUserAddressDto>().AsQueryable()
        };

        return View(model);
    }
}
public class CheckoutAddressViewModel
{
    public ResultUserAddressDto? ShippingAddress { get; set; }
    public ResultUserAddressDto? BillingAddress { get; set; }
    public required IQueryable<ResultUserAddressDto> AllAddresses { get; set; }
}
