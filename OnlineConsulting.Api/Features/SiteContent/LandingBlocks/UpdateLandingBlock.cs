using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.UpdateLandingBlock;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.LandingBlocks;

public class UpdateLandingBlock : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/site-content/landing-blocks/{id:guid}", Handle)
            .WithTags("SiteContent/LandingBlocks")
            .RequireAuthorization()
            .WithName("UpdateLandingBlock")
            .WithDescription("Updates a platform landing block (SuperAdmin).")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateLandingBlockRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateLandingBlockRequest(string Section, string Title, string Body, string? Icon = null, int DisplayOrder = 0)
{
    public UpdateLandingBlockCommand ToCommand(Guid id) => new(id, Section, Title, Body, Icon, DisplayOrder);
}
