using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.CreateLandingBlock;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.LandingBlocks;

public class CreateLandingBlock : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/landing-blocks", Handle)
            .WithTags("SiteContent/LandingBlocks")
            .RequireAuthorization()
            .WithName("CreateLandingBlock")
            .WithDescription("Creates a platform landing feature, step or FAQ entry (SuperAdmin).")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateLandingBlockRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateLandingBlockRequest(string Section, string Title, string Body, string? Icon = null, int DisplayOrder = 0)
{
    public CreateLandingBlockCommand ToCommand() => new(Section, Title, Body, Icon, DisplayOrder);
}
