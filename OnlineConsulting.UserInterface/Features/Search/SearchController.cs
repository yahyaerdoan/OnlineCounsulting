using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Features.Search;

[AllowAnonymous]
[Route("search")]
public class SearchController(ISearchService searchService) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? q, CancellationToken cancellationToken)
    {
        ViewBag.Query = q;

        return View(await searchService.SearchAsync(q ?? string.Empty, cancellationToken));
    }
}
