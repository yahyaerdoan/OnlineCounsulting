using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.Admin.Features.WhatWeProvide;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeWhatWeProvideViewComponents;

public class HomeWhatWeProvideProvideImageComponentPartial(IWhatWeProvideService whatWeProvideService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = await whatWeProvideService.GetAllAsync();
        return View(items);
    }
}
