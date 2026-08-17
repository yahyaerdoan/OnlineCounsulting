using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Testimonial;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeTestimonialsViewComponents;

public class HomeTestimonialsComponentPartial(ITestimonialService testimonialService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = await testimonialService.GetAllAsync();
        return View(items);
    }
}
