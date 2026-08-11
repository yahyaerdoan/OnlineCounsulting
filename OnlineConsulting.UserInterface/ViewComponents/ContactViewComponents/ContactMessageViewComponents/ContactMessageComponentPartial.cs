using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.MessageDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.ContactViewComponents.ContactMessageViewComponents;

public class ContactMessageComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var model = new CreateMessageDto();
        return View(model);
    }
}
