using ResultHandler.AspNetCore.Extensions;
using OnlineConsulting.Api.Common;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.CategoryDtos;

namespace OnlineConsulting.Api.Features.Categories;

public class GetCategoriesRawProblemDetails : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories/raw-problem-details", Handle)
            .WithTags("Categories")
            .RequireAuthorization()
            .WithName("GetCategoriesRawProblemDetails")
            .WithDescription("Returns all active categories; on failure, builds the raw ProblemDetails POCO instead of using ToProblemResult.");
    }

    private static async Task<IResult> Handle(IServiceManager serviceManager, HttpContext httpContext)
    {
        var result = await serviceManager.CategoryService.GetAllAsync<ResultCategoryDto>(false);
        if (!result.IsSuccessful)
        {
            var problem = result.ToProblemDetails(httpContext);
            return Results.Json(problem, statusCode: problem.Status, contentType: "application/problem+json");
        }

        return Results.Ok(result.Data);
    }
}
