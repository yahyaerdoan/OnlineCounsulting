using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Categories.Application.Features.Categories.UpdateCategory;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Categories;

public class UpdateCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/categories/{id:guid}", Handle)
            .WithTags("Categories")
            .RequireAuthorization()
            .WithName("UpdateCategory")
            .WithDescription("Updates an existing category.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateCategoryRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateCategoryRequest(string Title, string Description, string Icon, string? IconColor = null)
{
    public UpdateCategoryCommand ToCommand(Guid id) => new(id, Title, Description, Icon, IconColor);
}
