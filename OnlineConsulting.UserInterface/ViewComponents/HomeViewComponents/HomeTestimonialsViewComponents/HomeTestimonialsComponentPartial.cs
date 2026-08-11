using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.TestimonialDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeTestimonialsViewComponents;

public class HomeTestimonialsComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.TestimonialService.GetAllAsync<ResultTestimonialDto>(false, true);
        return View(result.Data ?? Enumerable.Empty<ResultTestimonialDto>().AsQueryable());
    }
}
