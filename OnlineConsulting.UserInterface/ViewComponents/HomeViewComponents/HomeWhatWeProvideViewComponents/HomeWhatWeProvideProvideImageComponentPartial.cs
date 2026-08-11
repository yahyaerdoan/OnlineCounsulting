using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.WhatWeProvideDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeWhatWeProvideViewComponents;

public class HomeWhatWeProvideProvideImageComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.WhatWeProvideService.GetAllAsync<ResultWhatWeProvideDto>(false, true);
        return View(result.Data ?? Enumerable.Empty<ResultWhatWeProvideDto>().AsQueryable());
    }
}
