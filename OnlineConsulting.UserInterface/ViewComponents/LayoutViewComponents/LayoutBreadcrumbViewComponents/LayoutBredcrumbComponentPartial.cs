using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Breadcrumb;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.LayoutBreadcrumbViewComponents;

public class LayoutBredcrumbComponentPartial(IBreadcrumbService breadcrumbService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = await breadcrumbService.GetAllAsync();
        return View(items);
    }
}
