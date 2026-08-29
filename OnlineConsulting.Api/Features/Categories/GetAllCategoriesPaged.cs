using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Categories.Application.Features.GetAllCategoriesPaged;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Categories;

public class GetAllCategoriesPaged : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/categories/query", Handle)
            .WithTags("Categories")
            .WithName("GetAllCategoriesPaged")
            .WithDescription("Returns categories, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new GetAllCategoriesPagedQuery(query.ToPageRequest(), dynamicQuery));
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
