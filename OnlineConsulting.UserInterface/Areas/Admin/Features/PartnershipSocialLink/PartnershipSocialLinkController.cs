using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.PartnershipSocialLink;

[Area("Admin")]
[Route("admin/partnerships/{partnershipId:guid}/social-links")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class PartnershipSocialLinkController(IPartnershipSocialLinkService partnershipSocialLinkService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(Guid partnershipId, CancellationToken cancellationToken)
    {
        ViewBag.PartnershipId = partnershipId;
        return View(await partnershipSocialLinkService.GetAllByPartnershipAsync(partnershipId, cancellationToken));
    }

    [HttpGet("create")]
    public IActionResult Create(Guid partnershipId)
    {
        ViewBag.PartnershipId = partnershipId;
        return View(new CreatePartnershipSocialLinkViewModel { PartnershipId = partnershipId });
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(Guid partnershipId, CreatePartnershipSocialLinkViewModel model, CancellationToken cancellationToken)
    {
        ViewBag.PartnershipId = partnershipId;
        model.PartnershipId = partnershipId;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await partnershipSocialLinkService.CreateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index", new { partnershipId }) : View(model);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid partnershipId, Guid id, CancellationToken cancellationToken)
    {
        ViewBag.PartnershipId = partnershipId;
        var model = await partnershipSocialLinkService.GetByIdAsync(partnershipId, id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid partnershipId, UpdatePartnershipSocialLinkViewModel model, CancellationToken cancellationToken)
    {
        ViewBag.PartnershipId = partnershipId;
        model.PartnershipId = partnershipId;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await partnershipSocialLinkService.UpdateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index", new { partnershipId }) : View(model);
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid partnershipId, Guid id, CancellationToken cancellationToken)
    {
        var result = await partnershipSocialLinkService.DeleteAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index", new { partnershipId });
    }
}
