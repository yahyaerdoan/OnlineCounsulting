using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ContactDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.ContactViewComponents.ContactInformationViewComponents;

public class ContactInformationComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.ContactService.GetAllAsync<ResultContactDto>(false);
        return View(result.Data ?? Enumerable.Empty<ResultContactDto>().AsQueryable());
    }
}
