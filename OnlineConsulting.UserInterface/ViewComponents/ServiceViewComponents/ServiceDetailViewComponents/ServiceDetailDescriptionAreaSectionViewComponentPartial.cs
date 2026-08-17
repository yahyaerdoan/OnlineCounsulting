using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Service;

namespace OnlineConsulting.UserInterface.ViewComponents.ServiceViewComponents.ServiceDetailViewComponents;

/// <summary>Was previously a sync Invoke() blocking on .Result (sync-over-async) - fixed to proper InvokeAsync
/// while rewiring, since the old blocking pattern is a genuine bug, not something worth preserving.</summary>
public class ServiceDetailDescriptionAreaSectionViewComponentPartial(IServiceCatalogPageService serviceCatalogPageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string slug)
    {
        var model = await serviceCatalogPageService.GetDetailAsync(slug);
        return View(model);
    }
}
