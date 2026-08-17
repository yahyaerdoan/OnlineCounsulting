using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Contact;

namespace OnlineConsulting.UserInterface.ViewComponents.ServiceViewComponents.ServiceDetailViewComponents;

/// <summary>Just hands the view an empty form model - CreateMessageViewModel (Features/Contact) replaces the old
/// CreateMessageDto so it binds straight into IContactService via ServiceController's Detail POST.</summary>
public class ServiceDetailContactAboutTheServiceAreaSectionViewComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View(new CreateMessageViewModel());
}
