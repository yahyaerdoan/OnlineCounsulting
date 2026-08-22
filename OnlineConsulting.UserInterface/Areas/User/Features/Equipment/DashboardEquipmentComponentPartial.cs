using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Equipment;

public class DashboardEquipmentComponentPartial(IUserEquipmentPageService pageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() =>
        View(await pageService.GetMyEquipmentAsync());
}
