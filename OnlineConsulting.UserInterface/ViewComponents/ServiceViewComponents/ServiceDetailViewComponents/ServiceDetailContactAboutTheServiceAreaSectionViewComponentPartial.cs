using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Contact;

namespace OnlineConsulting.UserInterface.ViewComponents.ServiceViewComponents.ServiceDetailViewComponents;

/// <summary>Hands the view an empty contact form model, bound via ServiceController's Detail POST.</summary>
public class ServiceDetailContactAboutTheServiceAreaSectionViewComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View(new CreateMessageViewModel());
}
