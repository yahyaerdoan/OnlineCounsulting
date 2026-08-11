using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.HeaderViewComponents.HeaderTopAreaViewComponents;

public class LayoutHeaderLoginAndRegisterComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userResult = await serviceManager.SystemUserService.GetCurrentUserAsync();

        if (userResult?.Data is not null)  // user ve ResultData null değilse devam et
        {
            ViewBag.userFullName = $"{userResult.Data.FirstName} {userResult.Data.LastName}";
        }
        else
        {
            ViewBag.userFullName = null;  // Null hatasını önlemek için boş string atanıyor
        }

        return View();
    }
}
