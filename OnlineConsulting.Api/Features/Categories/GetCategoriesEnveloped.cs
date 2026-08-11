using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Mapping;
using OnlineConsulting.Api.Common;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.CategoryDtos;

namespace OnlineConsulting.Api.Features.Categories;

public class GetCategoriesEnveloped : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories/enveloped", Handle)
            .WithTags("Categories")
            .RequireAuthorization()
            .WithName("GetCategoriesEnveloped")
            .WithDescription("Returns all active categories wrapped in the full result envelope (data + status + title + errors).");
    }

    private static async Task<IResult> Handle(IServiceManager serviceManager, HttpContext httpContext)
    {
        var result = await serviceManager.CategoryService.GetAllAsync<ResultCategoryDto>(false);
        return result.IsSuccessful
            ? Results.Json(result, statusCode: (int)result.Status.ToHttpStatusCode())
            : result.ToProblemResult(httpContext);
    }
}
