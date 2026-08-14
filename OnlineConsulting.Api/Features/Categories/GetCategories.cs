using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Categories.Application.Features.GetCategories;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Categories;

public class GetCategories : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories", Handle)
            .WithTags("Categories")
            .RequireAuthorization()
            .WithName("GetCategories")
            .WithDescription("Returns the current tenant's categories, paginated.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetCategoriesQuery(PageRequestFactory.Create(index, size)));
        return result
            .OnSuccess(page =>
            {
                foreach (var category in page.Items)
                {
                    category.Links = GetCategoryById.BuildLinks(httpContext, linkGenerator, category.Id);
                }
            })
            .ToEnvelopedResult(httpContext);
    }
}
