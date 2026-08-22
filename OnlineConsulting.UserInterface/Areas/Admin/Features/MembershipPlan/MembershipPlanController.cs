using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.MembershipPlan;

[Area("Admin")]
[Route("admin/membership-plans")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class MembershipPlanController(IMembershipPlanService membershipPlanService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await membershipPlanService.GetAllAsync(cancellationToken));

    [HttpGet("create")]
    public IActionResult Create() => View(new CreateMembershipPlanViewModel());

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateMembershipPlanViewModel createViewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(createViewModel);
        }

        var result = await membershipPlanService.CreateAsync(createViewModel, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(createViewModel);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
    {
        var model = await membershipPlanService.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(UpdateMembershipPlanViewModel updateViewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(updateViewModel);
        }

        var result = await membershipPlanService.UpdateAsync(updateViewModel, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(updateViewModel);
    }

    [HttpGet("subscribers")]
    public async Task<IActionResult> Subscribers(CancellationToken cancellationToken) =>
        View(await membershipPlanService.GetSubscribersAsync(cancellationToken));
}
