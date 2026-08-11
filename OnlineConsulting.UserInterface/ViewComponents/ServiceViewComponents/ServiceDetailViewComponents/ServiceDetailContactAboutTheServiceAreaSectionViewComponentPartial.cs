using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.MessageDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.ServiceViewComponents.ServiceDetailViewComponents;

public class ServiceDetailContactAboutTheServiceAreaSectionViewComponentPartial() : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var model = new CreateMessageDto();
        return View(model);
    }
}
