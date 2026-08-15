using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.ServiceViewComponents.ServiceDetailViewComponents;

public class ServiceDetailRelatedServiceAreaSectionViewComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string slug)
    {
        // Look up by slug, not ID - callers only have the slug here.
        var serviceResult = await serviceManager.ServiceService.GetServiceBySlugAsync<ResultServiceWithImageDto>(slug, false, true);

        if (!serviceResult.IsSuccessful || serviceResult.Data is null)
            return View(new List<ResultServiceWithImageDto>());

        var currentService = serviceResult.Data;

        var relatedServicesResult = await serviceManager.ServiceService
            .GetServicesWithImagesByCategoryIdAsync<ResultServiceWithImageDto>(currentService.CategoryId.ToString(), false, true);

        var filteredServices = (relatedServicesResult.Data ?? [])
            .Where(x => x.Id != currentService.Id)
            .ToList();

        return View(filteredServices);
    }
}
