using Hateoas.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Categories.Application.Features.Categories.CreateCategory;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Categories;

public class CreateCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/categories", Handle)
            .WithTags("Categories")
            .RequireAuthorization()
            .WithName("CreateCategory")
            .WithCreatedLocation("GetCategoryById")
            .WithDescription("Creates a new category for the current tenant.")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateCategoryRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());

        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateCategoryRequest(string Title, string Description, string Icon, string? IconColor = null)
{
    public CreateCategoryCommand ToCommand() => new(Title, Description, Icon, IconColor);
}
