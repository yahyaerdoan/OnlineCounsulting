using Microsoft.AspNetCore.Mvc;
using AdminContact = OnlineConsulting.UserInterface.Areas.Admin.Features.Contact;
using OnlineConsulting.UserInterface.Areas.Admin.Features.SliderItem;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeSliderViewComponents;

public class HomeSliderComponentPartial(ISliderItemService sliderItemService, AdminContact.IContactService contactService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var itemsTask = sliderItemService.GetAllAsync();
        var contactTask = contactService.GetAsync();
        await Task.WhenAll(itemsTask, contactTask);

        return View(new HomeSliderViewModel(itemsTask.Result, contactTask.Result?.Phone));
    }
}

public record HomeSliderViewModel(List<SliderItemListItemViewModel> Slides, string? Phone);
