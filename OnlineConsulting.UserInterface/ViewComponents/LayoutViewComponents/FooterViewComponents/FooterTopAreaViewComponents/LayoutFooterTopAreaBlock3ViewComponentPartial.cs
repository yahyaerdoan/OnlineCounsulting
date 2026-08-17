using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Service;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.FooterViewComponents.FooterTopAreaViewComponents;

public class LayoutFooterTopAreaBlock3ViewComponentPartial(IServiceCatalogPageService serviceCatalogPageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        // The Api's ServiceCardViewModel doesn't carry CreatedDate (legacy "3 newest" ordering no longer
        // available) - closest real equivalent is the catalog's own first page, capped to 3 with a cover image.
        var page = await serviceCatalogPageService.GetPagedAsync(1, 3);
        var result = page.Services.Where(s => s.CoverImageUrl is not null).ToList();

        return View(result);
    }
}
