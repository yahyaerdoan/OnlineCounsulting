using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Service;

namespace OnlineConsulting.UserInterface.ViewComponents.ServiceViewComponents.ServiceServiceListViewComponents;

public class ServiceServiceListComponentPartial(IServiceCatalogPageService serviceCatalogPageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(int size, int page) =>
        View(await serviceCatalogPageService.GetPagedAsync(page, size));
}
