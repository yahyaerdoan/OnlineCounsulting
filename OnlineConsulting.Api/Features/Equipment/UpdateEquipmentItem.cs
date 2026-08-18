using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.UpdateEquipmentItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Equipment;

public class UpdateEquipmentItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/equipment/{id:guid}", Handle)
            .WithTags("Equipment")
            .RequireAuthorization()
            .WithName("UpdateEquipmentItem")
            .WithDescription("Updates a piece of a customer's installed equipment (admin/technician).");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateEquipmentItemCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
