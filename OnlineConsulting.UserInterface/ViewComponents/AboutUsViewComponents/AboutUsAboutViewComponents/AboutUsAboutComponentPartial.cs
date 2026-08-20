using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.Admin.Features.AboutUs;
using System.Web;

namespace OnlineConsulting.UserInterface.ViewComponents.AboutUsViewComponents.AboutUsAboutViewComponents;

/// <summary>No dedicated public IAboutUsService exists yet (the public AboutUsController is a static view with
/// no Api call of its own) - reuses the admin IAboutUsService like the Home page partials already do for their
/// admin-owned content services, since these are DTO/API concerns, not access-control.</summary>
public class AboutUsAboutComponentPartial(IAboutUsService aboutUsService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = await aboutUsService.GetAllAsync();
        var aboutUs = items.FirstOrDefault();

        if (aboutUs is null)
        {
            return Content(string.Empty);
        }

        var viewModel = new AboutUsAboutViewModel(aboutUs.Title, aboutUs.Description, ConvertToEmbededUrl(aboutUs.VideoUrl ?? string.Empty));

        return View(viewModel);
    }

    private static string ConvertToEmbededUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        var uri = new Uri(url);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var videoId = query["v"] ?? uri.Segments.LastOrDefault()?.Trim('/');
        return $"https://www.youtube.com/embed/{videoId}?autoplay=1&mute=1&loop=1&playlist={videoId}&rel=0&modestbranding=1";
    }
}

public record AboutUsAboutViewModel(string Title, string Description, string VideoUrl);
