using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Categories.Application.Features.UpdateCategory;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Categories;

public class UpdateCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/categories/{id:guid}", Handle)
            .WithTags("Categories")
            .RequireAuthorization()
            .WithName("UpdateCategory")
            .WithDescription("Updates an existing category.");
    }

    private static async Task<IResult> Handle(
        Guid id, [FromBody] UpdateCategoryCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
