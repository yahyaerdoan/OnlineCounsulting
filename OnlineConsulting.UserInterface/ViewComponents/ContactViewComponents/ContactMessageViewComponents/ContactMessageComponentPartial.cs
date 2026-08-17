using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Contact;

namespace OnlineConsulting.UserInterface.ViewComponents.ContactViewComponents.ContactMessageViewComponents;

public class ContactMessageComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var model = new CreateMessageViewModel();
        return View(model);
    }
}
