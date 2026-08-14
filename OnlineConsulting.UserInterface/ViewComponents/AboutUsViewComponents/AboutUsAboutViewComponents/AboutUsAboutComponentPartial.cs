using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.AboutUsDtos;
using System.Web;

namespace OnlineConsulting.UserInterface.ViewComponents.AboutUsViewComponents.AboutUsAboutViewComponents;

public class AboutUsAboutComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var resultData = await serviceManager.AboutUsService.GetAllAsync<ResultAboutUsDto>(false);
        var aboutUsData = resultData.Data?.FirstOrDefault();

        if (aboutUsData is null)
            return Content(string.Empty);

        aboutUsData.VideoUrl = ConvertToEmbededUrl(aboutUsData.VideoUrl);
        return View(aboutUsData);
    }

    private static string ConvertToEmbededUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return string.Empty;
        var uri = new Uri(url);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var videoId = query["v"] ?? uri.Segments.LastOrDefault()?.Trim('/');
        return $"https://www.youtube.com/embed/{videoId}?autoplay=1&mute=1&loop=1&playlist={videoId}&rel=0&modestbranding=1";
    }
}
