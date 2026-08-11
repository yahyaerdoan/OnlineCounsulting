using ResultHandler.AspNetCore.Extensions;
using OnlineConsulting.Api.Common;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.CategoryDtos;

namespace OnlineConsulting.Api.Features.Categories;

public class GetCategories : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories", Handle)
            .WithTags("Categories")
            .RequireAuthorization()
            .WithName("GetCategories")
            .WithDescription("Returns all active categories.");
    }

    private static async Task<IResult> Handle(IServiceManager serviceManager, HttpContext httpContext)
    {
        var result = await serviceManager.CategoryService.GetAllAsync<ResultCategoryDto>(false);
        return result.ToResult(httpContext);
    }
}
