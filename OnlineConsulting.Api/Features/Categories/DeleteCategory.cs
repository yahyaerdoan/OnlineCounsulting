using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Categories.Application.Features.DeleteCategory;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Categories;

public class DeleteCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/categories/{id:guid}", Handle)
            .WithTags("Categories")
            .RequireAuthorization()
            .WithName("DeleteCategory")
            .WithDescription("Soft-deletes a category.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteCategoryCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
