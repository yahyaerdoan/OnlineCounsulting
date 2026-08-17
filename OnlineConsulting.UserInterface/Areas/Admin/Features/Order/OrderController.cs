using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Order;

/// <summary>There is no create/update/delete endpoint for orders - an order is produced by checkout and the only
/// admin-side write is a refund, so the old screen's "Delete" button now refunds instead of destroying the record.</summary>
[Area("Admin")]
[Route("admin/orders")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class OrderController(IAdminOrderService adminOrderService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await adminOrderService.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}/refund")]
    public async Task<IActionResult> Refund(Guid id, CancellationToken cancellationToken)
    {
        var result = await adminOrderService.RefundAsync(id, cancellationToken: cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }
}
