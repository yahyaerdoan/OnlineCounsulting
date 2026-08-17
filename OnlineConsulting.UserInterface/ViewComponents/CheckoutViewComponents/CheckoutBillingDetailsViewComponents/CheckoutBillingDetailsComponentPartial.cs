using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;

namespace OnlineConsulting.UserInterface.ViewComponents.CheckoutViewComponents.CheckoutBillingDetailsViewComponents;

public class CheckoutBillingDetailsComponentPartial(IUserAddressService userAddressService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(Guid cartId)
    {
        ViewBag.cartId = cartId;

        var shipping = await userAddressService.GetShippingAsync();
        var billing = await userAddressService.GetBillingAsync();
        var all = await userAddressService.GetAllAsync();

        var model = new CheckoutAddressViewModel
        {
            ShippingAddress = shipping,
            BillingAddress = billing,
            AllAddresses = all,
        };

        return View(model);
    }
}

public class CheckoutAddressViewModel
{
    public UserAddressResponse? ShippingAddress { get; set; }
    public UserAddressResponse? BillingAddress { get; set; }
    public required List<UserAddressResponse> AllAddresses { get; set; }
}
