using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;

[Area("User")]
[Route("user/addresses")]
public class UserAddressController(IUserAddressPageService userAddressPageService, IToastNotification toastNotification) : Controller
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateUserAddressViewModel model, CancellationToken cancellationToken)
    {
        var result = await userAddressPageService.CreateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAddressTab();
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(UpdateUserAddressViewModel model, CancellationToken cancellationToken)
    {
        var result = await userAddressPageService.UpdateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAddressTab();
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await userAddressPageService.DeleteAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAddressTab();
    }

    private IActionResult RedirectToAddressTab() => RedirectToAction("Address", "Dashboard", new { area = "User" });
}
