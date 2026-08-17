using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;
using System.Net.Http.Headers;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Dashboard;

[Area("User")]
[Route("User/{controller}/{action}/{id?}")]
public class DashboardController(IApiClient apiClient, IToastNotification toastNotification) : Controller
{
    public IActionResult Index() => View();
    public IActionResult Order() => View();
    public IActionResult OrderDetail(Guid id) => View(id);
    public IActionResult Address() => View();
    public IActionResult Account() => View();
    public IActionResult Invoice(Guid id) => View(id);
    public IActionResult Download() => View();

    [HttpPost]
    public async Task<IActionResult> UpdateImageFile([FromForm] Guid Id, [FromForm] IFormFile image)
    {
        using var content = new MultipartFormDataContent();
        using var stream = image.OpenReadStream();
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
        content.Add(fileContent, "image", image.FileName);

        var result = await apiClient.PostFileAsync<object>("/api/users/me/image", content);
        toastNotification.ShowResult(result);
        return RedirectToAction("Account", "Dashboard", new { area = "user" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(UserAccountViewModel model)
    {
        var result = await apiClient.PutAsync("/api/users/me/password", new
        {
            model.ChangePassword.CurrentPassword,
            model.ChangePassword.NewPassword,
        });

        toastNotification.ShowResult(result);
        return RedirectToAction("Account", "Dashboard", new { area = "user" });
    }
}
