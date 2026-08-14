using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.BusinessLogic.Concretions.Configurations.AppSettingConfigurations.AppSettingOptions;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Controllers;

public class CheckoutController(IServiceManager serviceManager, ISender sender, IToastNotification toastNotification, IOptions<AppSettingStripeOption> stripeOptions) : Controller
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("/checkout/{cartId}")]
    public IActionResult Index(Guid cartId)
    {
        ViewBag.StripePublishableKey = stripeOptions.Value.PublishableKey;
        ViewBag.cartId = cartId;
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> Update(UpdateUserAddressDto updateUserAddressDto, Guid cartId)
    {
        var result = await serviceManager.UserAddressService.UpdateAsync(updateUserAddressDto);

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index", "Checkout", new { area = "", cartId });
    }
    [HttpPost]
    public async Task<IActionResult> PlaceOrder(Guid cartId)
    {
        var currentUserResult = await sender.Send(new GetCurrentUserQuery());
        if (!currentUserResult.IsSuccessful || currentUserResult.Data is null)
        {
            NToastService.Show(toastNotification, currentUserResult.Title, currentUserResult.Status);
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        var userId = currentUserResult.Data.Id;

        var createOrderDto = new CreateOrderDto()
        {
            UserId = userId,
        };
        var result = await serviceManager.OrderService.CreateOrderFromBasketAsync(createOrderDto);
        if (!result.IsSuccessful)
        {
            NToastService.Show(toastNotification, result.Title, result.Status);
            return RedirectToAction("Index", "Checkout", new { area = "", cartId });
        }

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Order", "Dashboard", new { area = "User" });
    }
    [HttpPost]
    public async Task<IActionResult> SetShippingAddress(string addressId, string oldAddressId, string cartId)
    {
        var result = await serviceManager.UserAddressService.SetShippingAddressAsync(new SetShippingAddressDto
        {
            AddressId = addressId,
            OldAddressId = oldAddressId
        });
        if (!result.IsSuccessful)
        {
            NToastService.Show(toastNotification, result.Title, result.Status);
            return RedirectToAction("Index", "checkout", new { cartId });
        }

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index", "checkout", new { cartId });
    }
    [HttpPost]
    public async Task<IActionResult> SetBillingAddress(string addressId, string oldAddressId, string cartId)
    {
        var result = await serviceManager.UserAddressService.SetBillingAddressAsync(new SetBillingAddressDto
        {
            AddressId = addressId,
            OldAddressId = oldAddressId
        });
        if (!result.IsSuccessful)
        {
            NToastService.Show(toastNotification, result.Title, result.Status);
            return RedirectToAction("Index", "checkout", new { cartId });
        }

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index", "checkout", new { cartId });
    }
}
