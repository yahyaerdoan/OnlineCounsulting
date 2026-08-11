using ResultHandler.AspNetCore.Extensions;
using OnlineConsulting.Api.Common;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.CategoryDtos;

namespace OnlineConsulting.Api.Features.Categories;

public class GetCategoriesManualSuccess : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories/manual-success", Handle)
            .WithTags("Categories")
            .RequireAuthorization()
            .WithName("GetCategoriesManualSuccess")
            .WithDescription("Returns all active categories; only the failure path is delegated to ResultHandler, the success body is built manually.");
    }

    private static async Task<IResult> Handle(IServiceManager serviceManager, HttpContext httpContext)
    {
        var result = await serviceManager.CategoryService.GetAllAsync<ResultCategoryDto>(false);
        if (!result.IsSuccessful)
            return result.ToProblemResult(httpContext);

        return Results.Ok(result.Data);
    }
}
