using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.UserInterface.Areas.User.ViewModels.UserViewModels;
using OnlineConsulting.UserInterface.Common;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardAccountDetailsViewComponents;

public class DashboardAccountDetailComponentPartial(ISender sender) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userResult = await sender.Send(new GetCurrentUserQuery());
        if (!userResult.IsSuccessful || userResult.Data is null)
            return Content(string.Empty);

        var viewModel = new UserAccountViewModel
        {
            User = userResult.Data.ToResultUserDto()
        };

        return View(viewModel);
    }
}
