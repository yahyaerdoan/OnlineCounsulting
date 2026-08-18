using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.WorkOrderMediaItems.AddWorkOrderMediaItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.WorkOrders.WorkOrderMediaItems;

public class AddWorkOrderMediaItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/work-orders/{workOrderId:guid}/media-items", Handle)
            .WithTags("Scheduling/WorkOrders")
            .RequireAuthorization()
            .WithName("AddWorkOrderMediaItem")
            .WithDescription("Attaches an already-uploaded photo/video to a work order's before/after gallery.");
    }

    private static async Task<IResult> Handle(Guid workOrderId, [FromBody] AddWorkOrderMediaItemCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { WorkOrderId = workOrderId });
        return result.ToEnvelopedResult(httpContext);
    }
}
