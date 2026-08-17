using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Service;

namespace OnlineConsulting.UserInterface.ViewComponents.ServiceViewComponents.ServiceDetailViewComponents;

public class ServiceDetailViewComponentPartial(IServiceCatalogPageService serviceCatalogPageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string slug)
    {
        var model = await serviceCatalogPageService.GetDetailAsync(slug);
        return model is null ? Content(string.Empty) : View(model);
    }
}
