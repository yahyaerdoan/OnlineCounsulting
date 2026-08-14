using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.HeaderViewComponents.HeaderTopAreaViewComponents;

public class LayoutHeaderLoginAndRegisterComponentPartial(ISender sender) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userResult = await sender.Send(new GetCurrentUserQuery());

        if (userResult?.Data is not null)  // user ve ResultData null deÄŸilse devam et
        {
            ViewBag.userFullName = $"{userResult.Data.FirstName} {userResult.Data.LastName}";
        }
        else
        {
            ViewBag.userFullName = null;  // Null hatasÄ±nÄ± Ã¶nlemek iÃ§in boÅŸ string atanÄ±yor
        }

        return View();
    }
}
