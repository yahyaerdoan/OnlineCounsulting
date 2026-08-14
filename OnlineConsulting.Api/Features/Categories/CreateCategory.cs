using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Categories.Application.Features.CreateCategory;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Categories;

public class CreateCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/categories", Handle)
            .WithTags("Categories")
            .RequireAuthorization()
            .WithName("CreateCategory")
            .WithDescription("Creates a new category for the current tenant.");
    }

    private static async Task<IResult> Handle(
        [FromBody] CreateCategoryCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
