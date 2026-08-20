using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.GetWorkOrdersByEquipmentId;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.WorkOrders;

public class GetWorkOrdersByEquipmentId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/equipment/{equipmentId:guid}/work-orders", Handle)
            .WithTags("Scheduling/WorkOrders")
            .RequireAuthorization()
            .WithName("GetWorkOrdersByEquipmentId")
            .WithDescription("Returns the service history (work orders) recorded against a piece of equipment - the equipment health panel's timeline.");
    }

    private static async Task<IResult> Handle(Guid equipmentId, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetWorkOrdersByEquipmentIdQuery(equipmentId));
        return result.ToEnvelopedResult(httpContext);
    }
}
