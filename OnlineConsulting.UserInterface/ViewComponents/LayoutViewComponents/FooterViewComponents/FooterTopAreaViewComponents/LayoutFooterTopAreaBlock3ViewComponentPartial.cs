using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Service;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.FooterViewComponents.FooterTopAreaViewComponents;

public class LayoutFooterTopAreaBlock3ViewComponentPartial(IServiceCatalogPageService serviceCatalogPageService) : ViewComponent
{
    /// <summary>Shows up to 3 catalog services with a cover image; no CreatedDate on the API model, so this uses the catalog's first page instead of true "newest".</summary>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var page = await serviceCatalogPageService.GetPagedAsync(1, 3);
        var result = page.Services.Where(s => s.CoverImageUrl is not null).ToList();

        return View(result);
    }
}
