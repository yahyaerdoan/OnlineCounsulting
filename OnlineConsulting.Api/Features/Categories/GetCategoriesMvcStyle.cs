using Microsoft.AspNetCore.Mvc;
using ResultHandler.AspNetCore.Extensions;
using OnlineConsulting.Api.Common;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.CategoryDtos;

namespace OnlineConsulting.Api.Features.Categories;

/// <remarks>
/// Reference only: returning <see cref="IActionResult"/> from a Minimal API delegate triggers
/// analyzer warning ASP0004 and loses compile-time OpenAPI metadata. Prefer <see cref="GetCategories"/>
/// (ToResult/IResult) in real Minimal API endpoints; this exists to show the MVC-surface equivalent.
/// </remarks>
public class GetCategoriesMvcStyle : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories/mvc-style", Handle)
            .WithTags("Categories")
            .RequireAuthorization()
            .WithName("GetCategoriesMvcStyle")
            .WithDescription("Returns all active categories using the MVC IActionResult surface (ToActionResult) instead of the Minimal API IResult surface.");
    }

#pragma warning disable ASP0004 // Intentional: demonstrates the MVC surface; real endpoints use GetCategories (IResult) instead.
    private static async Task<IActionResult> Handle(IServiceManager serviceManager, HttpContext httpContext)
    {
        var result = await serviceManager.CategoryService.GetAllAsync<ResultCategoryDto>(false);
        return result.ToActionResult(httpContext);
    }
#pragma warning restore ASP0004
}
