using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.AddServiceMediaItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Services;

public class AddServiceMediaItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/services/media-items", Handle)
            .WithTags("Services")
            .RequireAuthorization()
            .WithName("AddServiceMediaItem")
            .WithDescription("Attaches an already-uploaded photo or video to a service's gallery.");
    }

    private static async Task<IResult> Handle([FromBody] AddServiceMediaItemCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
