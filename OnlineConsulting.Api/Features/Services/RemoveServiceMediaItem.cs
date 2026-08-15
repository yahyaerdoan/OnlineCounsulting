using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.RemoveServiceMediaItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Services;

public class RemoveServiceMediaItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/services/media-items/{id:guid}", Handle)
            .WithTags("Services")
            .RequireAuthorization()
            .WithName("RemoveServiceMediaItem")
            .WithDescription("Removes a photo or video from a service's gallery.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new RemoveServiceMediaItemCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
