using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Controllers;

[AllowAnonymous]
public class CartController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userResult = await serviceManager.SystemUserService.GetCurrentUserAsync();
        if (!userResult.IsSuccessful || userResult.Data is null)
        {
            NToastService.Show(toastNotification, userResult.Title, userResult.Status);
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Cart") });
        }

        var userId = userResult.Data.Id;

        var result = await serviceManager.BasketItemService.GetBasketItemsByUserIdAsync(userId, false, true);
        return View(result.Data);
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("/cart/add/{slug}")]
    public async Task<IActionResult> AddToCart(string slug)
    {
        // Slug ile servisi getir
        var serviceResult = await serviceManager.ServiceService.GetServiceBySlugAsync<ResultServiceWithImageDto>(slug, false, true);

        if (!serviceResult.IsSuccessful || serviceResult.Data is null)
        {
            NToastService.Show(toastNotification, serviceResult.Title, serviceResult.Status);
            return RedirectToAction("Index", "Home");
        }

        // ID'yi al ve sepete ekle
        var serviceId = serviceResult.Data.Id;
        var result = await serviceManager.BasketItemService.CreateBasketItemAsync(serviceId);
        if (!result.IsSuccessful)
        {
            NToastService.Show(toastNotification, result.Title, result.Status);
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("AddToCart", "Cart", new { slug }) });
        }

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index", "Service");
    }
    [HttpPost]
    public async Task<IActionResult> RemoveBasketItem(string id)
    {
        var userResult = await serviceManager.SystemUserService.GetCurrentUserAsync();
        if (!userResult.IsSuccessful)
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Cart") });

        var userId = userResult.Data.Id;

        var result = await serviceManager.BasketItemService.RemoveBasketItemAsync(userId, Guid.Parse(id));
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
}
