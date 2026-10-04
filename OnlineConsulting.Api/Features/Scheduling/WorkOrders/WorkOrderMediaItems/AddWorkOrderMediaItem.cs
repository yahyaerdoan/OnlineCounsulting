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
        _ = app.MapPost("/work-orders/{workOrderId:guid}/media-items", Handle)
            .WithTags("Scheduling/WorkOrders")
            .RequireAuthorization()
            .WithName("AddWorkOrderMediaItem")
            .WithDescription("Attaches an already-uploaded photo/video to a work order's before/after gallery.");
    }

    private static async Task<IResult> Handle(Guid workOrderId, [FromBody] AddWorkOrderMediaItemRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(workOrderId));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record AddWorkOrderMediaItemRequest(Guid MediaAssetId, bool IsBeforePhoto, int DisplayOrder = 0)
{
    public AddWorkOrderMediaItemCommand ToCommand(Guid workOrderId) => new(workOrderId, MediaAssetId, IsBeforePhoto, DisplayOrder);
}
