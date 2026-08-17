using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Service;

namespace OnlineConsulting.UserInterface.ViewComponents.ServiceViewComponents.ServiceDetailViewComponents;

public class ServiceDetailRelatedServiceAreaSectionViewComponentPartial(IServiceCatalogPageService serviceCatalogPageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string slug) =>
        View(await serviceCatalogPageService.GetRelatedAsync(slug));
}
