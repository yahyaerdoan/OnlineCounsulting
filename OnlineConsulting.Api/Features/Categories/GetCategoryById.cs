using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Categories.Application.Features.Categories.GetCategoryById;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Categories;

public class GetCategoryById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/categories/{id:guid}", Handle)
            .WithTags("Categories")
            .WithName("GetCategoryById")
            .WithDescription("Returns a single category by id.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
