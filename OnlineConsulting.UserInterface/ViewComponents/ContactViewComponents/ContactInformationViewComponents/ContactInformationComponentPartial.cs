using Microsoft.AspNetCore.Mvc;
using AdminContact = OnlineConsulting.UserInterface.Areas.Admin.Features.Contact;

namespace OnlineConsulting.UserInterface.ViewComponents.ContactViewComponents.ContactInformationViewComponents;

public class ContactInformationComponentPartial(AdminContact.IContactService contactService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var contact = await contactService.GetAsync();
        var items = contact is null ? [] : new List<AdminContact.ContactViewModel> { contact };

        return View(items);
    }
}
