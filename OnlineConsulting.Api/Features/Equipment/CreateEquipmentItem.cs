using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.CreateEquipmentItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Equipment;

public class CreateEquipmentItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/equipment", Handle)
            .WithTags("Equipment")
            .RequireAuthorization()
            .WithName("CreateEquipmentItem")
            .WithDescription("Records a piece of a customer's installed equipment (admin/technician).");
    }

    private static async Task<IResult> Handle([FromBody] CreateEquipmentItemCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
