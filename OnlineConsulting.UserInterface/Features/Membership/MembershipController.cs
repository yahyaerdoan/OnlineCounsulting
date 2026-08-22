using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Features.Membership;

[AllowAnonymous]
[Route("membership-plans")]
public class MembershipController(IMembershipPlanCatalogService catalogService) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await catalogService.GetAllAsync(cancellationToken));
}
