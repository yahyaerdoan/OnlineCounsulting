using Hateoas;
using Hateoas.AspNetCore;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Categories.Application.Features.GetCategoryById;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Categories;

public class GetCategoryById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories/{id:guid}", Handle)
            .WithTags("Categories")
            .RequireAuthorization()
            .WithName("GetCategoryById")
            .WithDescription("Returns a single category by id.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, LinkGenerator linkGenerator, HttpContext httpContext)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id));
        return result
            .OnSuccess(category => category.Links = BuildLinks(httpContext, linkGenerator, category.Id))
            .ToEnvelopedResult(httpContext);
    }

    internal static Dictionary<string, Link> BuildLinks(HttpContext httpContext, LinkGenerator linkGenerator, Guid id)
        => httpContext.Links(linkGenerator)
            .Add("self", "GetCategoryById", "GET", new { id })
            .Add("edit", "UpdateCategory", "PUT", new { id })
            .AddCustom("delete", "DeleteCategory", "DELETE", new { id })
            .Build();
}
